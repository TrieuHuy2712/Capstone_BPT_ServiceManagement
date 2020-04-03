using BPT_Service.Application.EmailService.Query.GetAllEmailService;
using BPT_Service.Application.PermissionService.Query.CheckUserIsAdmin;
using BPT_Service.Application.PermissionService.Query.GetPermissionAction;
using BPT_Service.Application.ProviderService.Query.CheckUserIsProvider;
using BPT_Service.Application.ProviderService.ViewModel;
using BPT_Service.Common;
using BPT_Service.Common.Constants.EmailConstant;
using BPT_Service.Common.Dtos;
using BPT_Service.Common.Helpers;
using BPT_Service.Common.Logging;
using BPT_Service.Model.Entities;
using BPT_Service.Model.Entities.ServiceModel;
using BPT_Service.Model.Enums;
using BPT_Service.Model.Infrastructure.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using SendGrid;
using SendGrid.Helpers.Mail;
using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace BPT_Service.Application.ProviderService.Command.RejectProviderService
{
    public class RejectProviderServiceCommand : IRejectProviderServiceCommand
    {
        private readonly ICheckUserIsAdminQuery _checkUserIsAdminQuery;
        private readonly ICheckUserIsProviderQuery _checkUserIsProviderQuery;
        private readonly IGetAllEmailServiceQuery _getAllEmailServiceQuery;
        private readonly IGetPermissionActionQuery _getPermissionActionQuery;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IOptions<EmailConfigModel> _config;
        private readonly IRepository<Provider, Guid> _providerRepository;
        private readonly UserManager<AppUser> _userRepository;

        public RejectProviderServiceCommand(
            ICheckUserIsAdminQuery checkUserIsAdminQuery,
            ICheckUserIsProviderQuery checkUserIsProviderQuery,
            IGetAllEmailServiceQuery getAllEmailServiceQuery,
            IGetPermissionActionQuery getPermissionActionQuery,
            IHttpContextAccessor httpContextAccessor,
            IOptions<EmailConfigModel> config,
            IRepository<Provider, Guid> providerRepository,
            UserManager<AppUser> userRepository)
        {
            _checkUserIsAdminQuery = checkUserIsAdminQuery;
            _checkUserIsProviderQuery = checkUserIsProviderQuery;
            _getAllEmailServiceQuery = getAllEmailServiceQuery;
            _getPermissionActionQuery = getPermissionActionQuery;
            _httpContextAccessor = httpContextAccessor;
            _config = config;
            _providerRepository = providerRepository;
            _userRepository = userRepository;
        }

        public async Task<CommandResult<ProviderServiceViewModel>> ExecuteAsync(string providerId, string reason)
        {
            var userName = _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value;
            try
            {
                var userId = _httpContextAccessor.HttpContext.User.Identity.Name;
                if (await _checkUserIsAdminQuery.ExecuteAsync(userId) || await _getPermissionActionQuery.ExecuteAsync(userId, "PROVIDER", ActionSetting.CanUpdate))
                {
                    var mappingProvider = await _providerRepository.FindByIdAsync(Guid.Parse(providerId));
                    if (mappingProvider == null)
                    {
                        return new CommandResult<ProviderServiceViewModel>
                        {
                            isValid = false,
                            errorMessage = ErrorMessageConstant.ERROR_CANNOT_FIND_ID
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
                    mappingProvider.Status = Status.InActive;
                    _providerRepository.Update(mappingProvider);
                    await _providerRepository.SaveAsync();
                    var userMail = await _userRepository.FindByIdAsync(mappingProvider.UserId.ToString());

                    //Set content for email
                    var getEmailContent = await _getAllEmailServiceQuery.ExecuteAsync();
                    var getFirstEmail = getEmailContent.Where(x => x.Name == EmailName.Reject_Provider).FirstOrDefault();
                    getFirstEmail.Message = getFirstEmail.Message.Replace(EmailKey.UserNameKey, userMail.Email).Replace(EmailKey.ReasonKey, reason);
                    ContentEmail(_config.Value.SendGridKey, getFirstEmail.Subject,
                                    getFirstEmail.Message, mappingProvider.AppUser.Email).Wait();

                    await LoggingUser<RejectProviderServiceCommand>.
                   InformationAsync(mappingProvider.UserId.ToString(), userName, userName + "Your provider:" + mappingProvider.ProviderName + "has been rejecte.Please check your email");
                    await Logging<RejectProviderServiceCommand>.
                        InformationAsync(ActionCommand.COMMAND_REJECT, userName, JsonConvert.SerializeObject(mappingProvider));
                    return new CommandResult<ProviderServiceViewModel>
                    {
                        isValid = true,
                    };
                }
                else
                {
                    await Logging<RejectProviderServiceCommand>.
                       WarningAsync(ActionCommand.COMMAND_REJECT, userName, ErrorMessageConstant.ERROR_UPDATE_PERMISSION);
                    return new CommandResult<ProviderServiceViewModel>
                    {
                        isValid = false,
                        errorMessage = ErrorMessageConstant.ERROR_UPDATE_PERMISSION
                    };
                }
            }
            catch (Exception ex)
            {
                await Logging<RejectProviderServiceCommand>.
                       ErrorAsync(ex, ActionCommand.COMMAND_REJECT, userName, "Has error");
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