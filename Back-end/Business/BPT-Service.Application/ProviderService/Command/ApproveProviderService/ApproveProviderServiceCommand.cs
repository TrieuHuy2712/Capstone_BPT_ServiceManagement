using BPT_Service.Application.EmailService.Query.GetAllEmailService;
using BPT_Service.Application.PermissionService.Query.CheckUserIsAdmin;
using BPT_Service.Application.PermissionService.Query.GetPermissionAction;
using BPT_Service.Application.PostService.Command.PostServiceFromUser.DeleteServiceFromUser;
using BPT_Service.Application.PostService.Query.GetPostUserServiceByUserId;
using BPT_Service.Application.ProviderService.Query.CheckUserIsProvider;
using BPT_Service.Application.ProviderService.ViewModel;
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

namespace BPT_Service.Application.ProviderService.Command.ApproveProviderService
{
    public class ApproveProviderServiceCommand : IApproveProviderServiceCommand
    {
        private readonly ICheckUserIsAdminQuery _checkUserIsAdminQuery;
        private readonly ICheckUserIsProviderQuery _checkUserIsProviderQuery;
        private readonly IDeleteServiceFromUserCommand _deleteServiceFromUserCommand;
        private readonly IGetAllEmailServiceQuery _getAllEmailServiceQuery;
        private readonly IGetPermissionActionQuery _getPermissionActionQuery;
        private readonly IGetPostUserServiceByUserIdQuery _getPostUserServiceByUserIdQuery;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IOptions<EmailConfigModel> _config;
        private readonly IRepository<Provider, Guid> _providerRepository;
        private readonly UserManager<AppUser> _userRepository;

        public ApproveProviderServiceCommand(
            IHttpContextAccessor httpContextAccessor,
            IRepository<Provider, Guid> providerRepository,
            UserManager<AppUser> userRepository,
            ICheckUserIsProviderQuery checkUserIsProviderQuery,
            IGetAllEmailServiceQuery getAllEmailServiceQuery,
            IOptions<EmailConfigModel> config,
            IGetPostUserServiceByUserIdQuery getPostUserServiceByUserIdQuery,
            IDeleteServiceFromUserCommand deleteServiceFromUserCommand,
            ICheckUserIsAdminQuery checkUserIsAdminQuery,
            IGetPermissionActionQuery getPermissionActionQuery)
        {
            _httpContextAccessor = httpContextAccessor;
            _providerRepository = providerRepository;
            _userRepository = userRepository;
            _checkUserIsProviderQuery = checkUserIsProviderQuery;
            _getAllEmailServiceQuery = getAllEmailServiceQuery;
            _config = config;
            _getPostUserServiceByUserIdQuery = getPostUserServiceByUserIdQuery;
            _deleteServiceFromUserCommand = deleteServiceFromUserCommand;
            _checkUserIsAdminQuery = checkUserIsAdminQuery;
            _getPermissionActionQuery = getPermissionActionQuery;
        }

        public async Task<CommandResult<ProviderServiceViewModel>> ExecuteAsync(string userProvider, string providerId)
        {
            try
            {
                var userId = _httpContextAccessor.HttpContext.User.Identity.Name;
                if(await _checkUserIsAdminQuery.ExecuteAsync(userId) || await _getPermissionActionQuery.ExecuteAsync(userId,"PROVIDER", ActionSetting.CanUpdate))
                {
                    var getUserService = await _getPostUserServiceByUserIdQuery.ExecuteAsync(userProvider);
                    if (getUserService != null)
                    {
                        await _deleteServiceFromUserCommand.ExecuteAsync(getUserService.Id.ToString());
                    }

                    var mappingProvider = await _providerRepository.FindByIdAsync(Guid.Parse(providerId));
                    if (mappingProvider == null)
                    {
                        return new CommandResult<ProviderServiceViewModel>
                        {
                            isValid = false,
                            errorMessage = "Cannot find your id provider"
                        };
                    }
                    //Check user is Provider
                    if (_checkUserIsProviderQuery.ExecuteAsync().Result.isValid == true)
                    {
                        return new CommandResult<ProviderServiceViewModel>
                        {
                            errorMessage = "You had been a provider"
                        };
                    }
                    mappingProvider.Status = Status.Active;
                    _providerRepository.Update(mappingProvider);
                    var findUserId = await _userRepository.FindByIdAsync(userId);
                    await _userRepository.AddToRoleAsync(findUserId, "Provider");
                    await _providerRepository.SaveAsync();
                    var userMail = await _userRepository.FindByIdAsync(mappingProvider.UserId.ToString());

                    //Set content for email
                    //var content = "Your provider: " + userMail.Email + " has been approved. Please check in our system";
                    var getEmailContent = await _getAllEmailServiceQuery.ExecuteAsync();
                    var getFirstEmail = getEmailContent.Where(x => x.Name == EmailName.Approve_Provider).FirstOrDefault();
                    getFirstEmail.Message = getFirstEmail.Message.Replace(EmailKey.UserNameKey, userMail.Email);

                    ContentEmail(_config.Value.SendGridKey, getFirstEmail.Subject,
                                    getFirstEmail.Message, _userRepository.FindByIdAsync(userId).Result.Email).Wait();
                    return new CommandResult<ProviderServiceViewModel>
                    {
                        isValid = true,
                    };
                }
                else
                {
                    return new CommandResult<ProviderServiceViewModel>
                    {
                        isValid = false,
                        errorMessage = ErrorMessageConstant.ERROR_UPDATE_PERMISSION
                    };
                }
                
            }
            catch (Exception ex)
            {
                return new CommandResult<ProviderServiceViewModel>
                {
                    isValid = false,
                    errorMessage = ex.InnerException.ToString()
                };
            }
        }

        private async Task ContentEmail(string apiKey, string subject1, string message, string email)
        {
            var client = new SendGridClient(apiKey);
            var from = new EmailAddress(_config.Value.FromUserEmail, _config.Value.FullUserName);
            var subject = subject1;
            var to = new EmailAddress(email);
            var plainTextContent = message;
            var htmlContent = "<strong>" + message + "</strong>";
            var msg = MailHelper.CreateSingleEmail(from, to, subject, plainTextContent, htmlContent);
            var response = await client.SendEmailAsync(msg);
        }
    }
}