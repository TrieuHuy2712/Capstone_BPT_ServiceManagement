using BPT_Service.Application.EmailService.Query.GetAllEmailService;
using BPT_Service.Application.PermissionService.Query.GetPermissionAction;
using BPT_Service.Application.PostService.ViewModel;
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

namespace BPT_Service.Application.PostService.Command.ApprovePostService
{
    public class ApprovePostServiceCommand : IApprovePostServiceCommand
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

        public ApprovePostServiceCommand(
             IGetAllEmailServiceQuery getAllEmailServiceQuery,
             IGetPermissionActionQuery getPermissionActionQuery,
             IHttpContextAccessor httpContextAccessor,
             IOptions<EmailConfigModel> configEmail,
             IRepository<BPT_Service.Model.Entities.ServiceModel.ProviderServiceModel.ProviderService, int> providerServiceRepository,
             IRepository<BPT_Service.Model.Entities.ServiceModel.UserServiceModel.UserService, int> userServiceRepository,
             IRepository<Provider, Guid> providerRepository,
             IRepository<Service, Guid> postServiceRepository,
             UserManager<AppUser> userRepository
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
        }

        public async Task<CommandResult<PostServiceViewModel>> ExecuteAsync(string idService)
        {
            try
            {
                var userId = _httpContextAccessor.HttpContext.User.Identity.Name;
                if (!await _getPermissionActionQuery.ExecuteAsync(userId, "Service", ActionSetting.CanUpdate))
                {
                    return new CommandResult<PostServiceViewModel>
                    {
                        isValid = false,
                        errorMessage = "You don't have permission"
                    };
                }
                var idUser = "";
                var getCurrentPost = await _postServiceRepository.FindByIdAsync(Guid.Parse(idService));
                if (getCurrentPost != null)
                {
                    getCurrentPost.Status = Status.Active;
                    _postServiceRepository.Update(getCurrentPost);
                    await _postServiceRepository.SaveAsync();

                    //Check is UserService
                    var serviceInformation = await _userServiceRepository.FindSingleAsync(x => x.ServiceId == Guid.Parse(idService));
                    if (serviceInformation == null)
                    {
                        //Check is ProviderService
                        var providerInformation = await _providerServiceRepository.FindSingleAsync(x => x.ServiceId == Guid.Parse(idService));
                        if (providerInformation != null)
                        {
                            var getProviderInformation = await _providerRepository.FindSingleAsync(x => x.Id == providerInformation.ProviderId);
                            idUser = getProviderInformation.UserId.ToString();
                        }
                        else
                        {
                            return new CommandResult<PostServiceViewModel>
                            {
                                isValid = false,
                            };
                        }
                    }
                    else
                    {
                        idUser = serviceInformation.UserId.ToString();
                    }

                    if (idUser != "")
                    {
                        var findEmailUser = await _userRepository.FindByIdAsync(idUser);
                        if (findEmailUser != null)
                        {
                            //Set content for email
                            //Get All email
                            var getAllEmail = await _getAllEmailServiceQuery.ExecuteAsync();
                            var getFirstEmail = getAllEmail.Where(x => x.Name == EmailName.Approve_Service).FirstOrDefault();
                            getFirstEmail.Message= getFirstEmail.Message.Replace(EmailKey.ServiceNameKey, getCurrentPost.ServiceName).Replace(EmailKey.UserNameKey, findEmailUser.Email);

                            ContentEmail(_configEmail.Value.SendGridKey, getFirstEmail.Subject,
                                            getFirstEmail.Message, findEmailUser.Email).Wait();
                        }
                        else
                        {
                            return new CommandResult<PostServiceViewModel>
                            {
                                isValid = false,
                            };
                        }
                    }

                    return new CommandResult<PostServiceViewModel>
                    {
                        isValid = true,
                    };
                }
                return new CommandResult<PostServiceViewModel>
                {
                    isValid = false,
                    errorMessage = "Cannot find Id of PostService"
                };
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
    }
}