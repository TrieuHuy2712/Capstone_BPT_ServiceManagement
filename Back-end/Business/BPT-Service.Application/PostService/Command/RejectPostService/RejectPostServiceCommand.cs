using BPT_Service.Application.EmailService.Query.GetAllEmailService;
using BPT_Service.Application.PermissionService.Query.CheckUserIsAdmin;
using BPT_Service.Application.PermissionService.Query.GetPermissionAction;
using BPT_Service.Application.PostService.ViewModel;
using BPT_Service.Common;
using BPT_Service.Common.Constants.EmailConstant;
using BPT_Service.Common.Dtos;
using BPT_Service.Common.Helpers;
using BPT_Service.Model.Entities;
using BPT_Service.Model.Entities.ServiceModel;
using BPT_Service.Model.Enums;
using BPT_Service.Model.Infrastructure.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using SendGrid;
using SendGrid.Helpers.Mail;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace BPT_Service.Application.PostService.Command.RejectPostService
{
    public class RejectPostServiceCommand : IRejectPostServiceCommand
    {
        private readonly IGetAllEmailServiceQuery _getAllEmailServiceQuery;
        private readonly IGetPermissionActionQuery _getPermissionActionQuery;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IOptions<EmailConfigModel> _configEmail;
        private readonly IRepository<Model.Entities.ServiceModel.ProviderServiceModel.ProviderService, int> _providerServiceRepository;
        private readonly IRepository<Model.Entities.ServiceModel.UserServiceModel.UserService, int> _userServiceRepository;
        private readonly IRepository<Provider, Guid> _providerRepository;
        private readonly IRepository<Service, Guid> _postServiceRepository;
        private readonly UserManager<AppUser> _userRepository;
        private readonly ICheckUserIsAdminQuery _checkUserIsAdminQuery;

        public RejectPostServiceCommand(
            IGetAllEmailServiceQuery getAllEmailServiceQuery,
            IGetPermissionActionQuery getPermissionActionQuery,
            IHttpContextAccessor httpContextAccessor,
            IOptions<EmailConfigModel> configEmail,
            IRepository<Model.Entities.ServiceModel.ProviderServiceModel.ProviderService, int> providerServiceRepository,
            IRepository<Model.Entities.ServiceModel.UserServiceModel.UserService, int> userServiceRepository,
            IRepository<Provider, Guid> providerRepository,
            IRepository<Service, Guid> postServiceRepository,
            UserManager<AppUser> userRepository,
            ICheckUserIsAdminQuery checkUserIsAdminQuery
            )
        {
            _configEmail = configEmail;
            _getAllEmailServiceQuery = getAllEmailServiceQuery;
            _getPermissionActionQuery = getPermissionActionQuery;
            _httpContextAccessor = httpContextAccessor;
            _postServiceRepository = postServiceRepository;
            _providerRepository = providerRepository;
            _providerServiceRepository = providerServiceRepository;
            _userRepository = userRepository;
            _userServiceRepository = userServiceRepository;
            _checkUserIsAdminQuery = checkUserIsAdminQuery;
        }

        public async Task<CommandResult<PostServiceViewModel>> ExecuteAsync(string idService, string reason)
        {
            try
            {
                var userId = _httpContextAccessor.HttpContext.User.Identity.Name;
                //Check permission approve
                if (await _getPermissionActionQuery.ExecuteAsync(userId, "SERVICE", ActionSetting.CanUpdate) ||
                    await _checkUserIsAdminQuery.ExecuteAsync(userId))
                {
                    //Check have current post
                    var getCurrentPost = await _postServiceRepository.FindByIdAsync(Guid.Parse(idService));
                    if (getCurrentPost != null)
                    {
                        getCurrentPost.Status = Status.Active;
                        _postServiceRepository.Update(getCurrentPost);
                        await _postServiceRepository.SaveAsync();

                        var findEmailUser = await GetEmailUserAsync(getCurrentPost);
                        if (findEmailUser != ErrorMessageConstant.ERROR_CANNOT_FIND_ID)
                        {
                            //Set content for email
                            //Get all email
                            var getAllEmail = await _getAllEmailServiceQuery.ExecuteAsync();
                            var getFirstEmail = getAllEmail.Where(x => x.Name == EmailName.Reject_Service).FirstOrDefault();
                            getFirstEmail.Message = getFirstEmail.Message.Replace(EmailKey.ServiceNameKey, getCurrentPost.ServiceName)
                                .Replace(EmailKey.UserNameKey, findEmailUser)
                                .Replace(EmailKey.ReasonKey, reason);

                            ContentEmail(_configEmail.Value.SendGridKey, getFirstEmail.Subject,
                                            getFirstEmail.Message, findEmailUser).Wait();
                        }
                        else
                        {
                            return new CommandResult<PostServiceViewModel>
                            {
                                isValid = false,
                                errorMessage = "Cannot find email user"
                            };
                        }

                        return new CommandResult<PostServiceViewModel>
                        {
                            isValid = true,
                        };
                    }
                    return new CommandResult<PostServiceViewModel>
                    {
                        isValid = false,
                        errorMessage = ErrorMessageConstant.ERROR_CANNOT_FIND_ID
                    };
                }
                else
                {
                    return new CommandResult<PostServiceViewModel>
                    {
                        isValid = false,
                        errorMessage = ErrorMessageConstant.ERROR_UPDATE_PERMISSION
                    };
                }
            }
            catch (System.Exception ex)
            {
                return new CommandResult<PostServiceViewModel>
                {
                    isValid = false,
                    errorMessage = ex.InnerException.ToString()
                };
            }
        }

        private async Task ContentEmail(string apiKey, string subject1, string message, string email)
        {
            var client = new SendGridClient(apiKey);
            var from = new EmailAddress(_configEmail.Value.FromUserEmail, _configEmail.Value.FullUserName);
            var subject = subject1;
            var to = new EmailAddress(email);
            var plainTextContent = message;
            var htmlContent = "<strong>" + message + "</strong>";
            var msg = MailHelper.CreateSingleEmail(from, to, subject, plainTextContent, htmlContent);
            var response = await client.SendEmailAsync(msg);
        }

        private async Task<string> GetEmailUserAsync(Service service)
        {
            var informationUserService = await _userServiceRepository.FindSingleAsync(x => x.ServiceId == service.Id);
            if (informationUserService != null)
            {
                var getUser = await _userRepository.FindByIdAsync(informationUserService.UserId.ToString());
                return getUser.Email;
            }
            var informationProviderService = await _providerServiceRepository.FindSingleAsync(x => x.ServiceId == service.Id);
            if (informationProviderService != null)
            {
                var getProvider = await _providerRepository.FindSingleAsync(x => x.Id == informationProviderService.ProviderId);
                var getUser = await _userRepository.FindByIdAsync(getProvider.UserId.ToString());
                return getUser.Email;
            }
            return ErrorMessageConstant.ERROR_CANNOT_FIND_ID;
        }
    }
}