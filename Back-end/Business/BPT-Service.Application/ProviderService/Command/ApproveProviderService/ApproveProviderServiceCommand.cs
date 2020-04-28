using BPT_Service.Application.EmailService.Query.GetAllEmailService;
using BPT_Service.Application.PermissionService.Query.CheckUserIsAdmin;
using BPT_Service.Application.PermissionService.Query.GetPermissionAction;
using BPT_Service.Application.PostService.Command.PostServiceFromUser.DeleteServiceFromUser;
using BPT_Service.Application.PostService.Query.GetPostUserServiceByUserId;
using BPT_Service.Application.ProviderService.Query.CheckUserIsProvider;
using BPT_Service.Application.ProviderService.ViewModel;
using BPT_Service.Common;
using BPT_Service.Common.Constants;
using BPT_Service.Common.Constants.EmailConstant;
using BPT_Service.Common.Dtos;
using BPT_Service.Common.Helpers;
using BPT_Service.Common.Logging;
using BPT_Service.Common.Support;
using BPT_Service.Model.Entities;
using BPT_Service.Model.Entities.ServiceModel;
using BPT_Service.Model.Enums;
using BPT_Service.Model.Infrastructure.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using SendGrid;
using SendGrid.Helpers.Mail;
using System;
using System.Linq;
using System.Security.Claims;
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
        private readonly IConfiguration _configuration;

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
            IGetPermissionActionQuery getPermissionActionQuery,
            IConfiguration configuration)
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
            _configuration = configuration;
        }

        public async Task<CommandResult<ProviderServiceViewModel>> ExecuteAsync(string userProvider, string providerId)
        {
            var userId = _httpContextAccessor.HttpContext.User.Identity.Name;
            var userName = _userRepository.FindByIdAsync(userId).Result.UserName;
            try
            {
                if(await _checkUserIsAdminQuery.ExecuteAsync(userId) || await _getPermissionActionQuery.ExecuteAsync(userId, ConstantFunctions.PROVIDER, ActionSetting.CanUpdate))
                {
                    var getUserService = await _getPostUserServiceByUserIdQuery.ExecuteAsync(userProvider);
                    if (getUserService != null)
                    {
                        await _deleteServiceFromUserCommand.ExecuteAsync(getUserService.Id.ToString());
                    }

                    var mappingProvider = await _providerRepository.FindByIdAsync(Guid.Parse(providerId));
                    //Write Log
                    await Logging<ApproveProviderServiceCommand>.
                        WarningAsync(ActionCommand.COMMAND_APPROVE, userName, ErrorMessageConstant.ERROR_CANNOT_FIND_ID);
                    if (mappingProvider == null)
                    {
                        return new CommandResult<ProviderServiceViewModel>
                        {
                            isValid = false,
                            errorMessage = ErrorMessageConstant.ERROR_CANNOT_FIND_ID
                        };
                    }
                    //Check user is Provider
                    if (_checkUserIsProviderQuery.ExecuteAsync(userId).Result.isValid == true)
                    {
                        return new CommandResult<ProviderServiceViewModel>
                        {
                            isValid = false,
                            errorMessage = "You had been a provider"
                        };
                    }
                    mappingProvider.Status = Status.WaitingApprove;
                    _providerRepository.Update(mappingProvider);
                    var findUserId = await _userRepository.FindByIdAsync(userId);
                    await _userRepository.AddToRoleAsync(findUserId, "Provider");
                    await _providerRepository.SaveAsync();
                    var userMail = await _userRepository.FindByIdAsync(mappingProvider.UserId.ToString());

                    //Set content for email
                    var getEmailContent = await _getAllEmailServiceQuery.ExecuteAsync();
                    var getFirstEmail = getEmailContent.Where(x => x.Name == EmailName.Approve_Provider).FirstOrDefault();

                    var generateCode = _configuration.GetSection("Host").GetSection("LinkConfirmProvider") +
                        mappingProvider.OTPConfirm + '_' + mappingProvider.Id;
                    getFirstEmail.Message = getFirstEmail.Message.Replace(EmailKey.UserNameKey, userMail.Email).Replace(EmailKey.ConfirmLink, generateCode);

                    ContentEmail(_config.Value.SendGridKey, getFirstEmail.Subject,
                                    getFirstEmail.Message, userMail.Email).Wait();

                    //Write log
                    await LoggingUser<ApproveProviderServiceCommand>.
                    InformationAsync(mappingProvider.UserId.ToString(), userName, userName + "Your provider:" + mappingProvider.ProviderName + "has been approved");
                    await Logging<ApproveProviderServiceCommand>.
                        InformationAsync(ActionCommand.COMMAND_APPROVE, userName, mappingProvider.ProviderName + "has been approved");

                    return new CommandResult<ProviderServiceViewModel>
                    {
                        isValid = true,
                    };
                }
                else
                {
                    await Logging<ApproveProviderServiceCommand>.
                        WarningAsync(ActionCommand.COMMAND_APPROVE, userName, ErrorMessageConstant.ERROR_UPDATE_PERMISSION);
                    return new CommandResult<ProviderServiceViewModel>
                    {
                        isValid = false,
                        errorMessage = ErrorMessageConstant.ERROR_UPDATE_PERMISSION
                    };
                }
                
            }
            catch (Exception ex)
            {
                await Logging<ApproveProviderServiceCommand>.
                       ErrorAsync(ex, ActionCommand.COMMAND_APPROVE, userName, "Has error");
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