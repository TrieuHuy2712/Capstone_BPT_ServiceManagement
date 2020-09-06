using BPT_Service.Application.EmailService.Query.GetAllEmailService;
using BPT_Service.Application.NewsProviderService.ViewModel;
using BPT_Service.Application.PermissionService.Query.CheckUserIsAdmin;
using BPT_Service.Application.PermissionService.Query.GetPermissionAction;
using BPT_Service.Common;
using BPT_Service.Common.Constants;
using BPT_Service.Common.Constants.EmailConstant;
using BPT_Service.Common.Dtos;
using BPT_Service.Common.Helpers;
using BPT_Service.Common.Logging;
using BPT_Service.Model.Entities;
using BPT_Service.Model.Entities.ServiceModel;
using BPT_Service.Model.Entities.ServiceModel.ProviderServiceModel;
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

namespace BPT_Service.Application.NewsProviderService.Command.ApproveNewsProvider
{
    public class ApproveNewsProviderServiceCommand : IApproveNewsProviderServiceCommand
    {
        private readonly IGetAllEmailServiceQuery _getAllEmailServiceQuery;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IOptions<EmailConfigModel> _configEmail;
        private readonly IRepository<Provider, Guid> _providerRepository;
        private readonly IRepository<ProviderNew, int> _newProviderRepository;
        private readonly UserManager<AppUser> _userRepository;
        private readonly ICheckUserIsAdminQuery _checkUserIsAdminQuery;
        private readonly IGetPermissionActionQuery _getPermissionActionQuery;
        private readonly IConfiguration _configuration;

        public ApproveNewsProviderServiceCommand(
            IGetAllEmailServiceQuery getAllEmailServiceQuery,
            IHttpContextAccessor httpContextAccessor,
            IOptions<EmailConfigModel> configEmail,
            IRepository<Provider, Guid> providerRepository,
            IRepository<ProviderNew, int> newProviderRepository,
            UserManager<AppUser> userRepository,
            ICheckUserIsAdminQuery checkUserIsAdminQuery,
            IGetPermissionActionQuery getPermissionActionQuery,
            IConfiguration configuration)
        {
            _configEmail = configEmail;
            _getAllEmailServiceQuery = getAllEmailServiceQuery;
            _httpContextAccessor = httpContextAccessor;
            _newProviderRepository = newProviderRepository;
            _providerRepository = providerRepository;
            _userRepository = userRepository;
            _checkUserIsAdminQuery = checkUserIsAdminQuery;
            _getPermissionActionQuery = getPermissionActionQuery;
            _configuration = configuration;
        }

        public async Task<CommandResult<NewsProviderViewModel>> ExecuteAsync(int idNews)
        {
            var userId = _httpContextAccessor.HttpContext.User.Identity.Name;
            var userName = _userRepository.FindByIdAsync(userId).Result.UserName;
            try
            {//Check user has permission first
                if (await _checkUserIsAdminQuery.ExecuteAsync(userId) || await _getPermissionActionQuery.ExecuteAsync(userId, ConstantFunctions.NEWS, ActionSetting.CanUpdate))
                {
                    var mappingProvider = await _newProviderRepository.FindByIdAsync(idNews);
                    if (mappingProvider == null)
                    {
                        return new CommandResult<NewsProviderViewModel>
                        {
                            isValid = false,
                            errorMessage = ErrorMessageConstant.ERROR_CANNOT_FIND_ID
                        };
                    }
                    var map = MappingNewProvider(mappingProvider);
                    _newProviderRepository.Update(map);
                    await _newProviderRepository.SaveAsync();

                    var getProvider = await _providerRepository.FindSingleAsync(x => x.Id == mappingProvider.ProviderId);
                    var getEmail = await _userRepository.FindByIdAsync(getProvider.UserId.ToString());
                    //Set content for email
                    var getAllEmail = await _getAllEmailServiceQuery.ExecuteAsync();
                    var getFirstEmail = getAllEmail.Where(x => x.Name == EmailName.Approve_News).FirstOrDefault();
                    var generateCode = _configuration.GetSection("Host").GetSection("LinkConfirmNewsProvider").Value +
                       mappingProvider.CodeConfirm + '_' + mappingProvider.Id;
                    getFirstEmail.Message = getFirstEmail.Message.
                        Replace(EmailKey.UserNameKey, getEmail.UserName).
                        Replace(EmailKey.NewNameKey, mappingProvider.Title).
                        Replace(EmailKey.ConfirmLink, generateCode); ;
                        
                    ContentEmail(_configEmail.Value.SendGridKey, getFirstEmail.Subject,
                                    getFirstEmail.Message, getEmail.Email).Wait();
                    await LoggingUser<ApproveNewsProviderServiceCommand>.
                    InformationAsync(getProvider.UserId.ToString(), userName, userName + "Tin tức" + map.Title + "của bạn đã được chấp thuận");
                    await Logging<ApproveNewsProviderServiceCommand>.
                        InformationAsync(ActionCommand.COMMAND_APPROVE, userName, userName + "Your news provider:" + map.Title + "has been approved");
                    return new CommandResult<NewsProviderViewModel>
                    {
                        isValid = true,
                    };
                }
                else
                {
                    await Logging<ApproveNewsProviderServiceCommand>.
                        WarningAsync(ActionCommand.COMMAND_APPROVE, userName, ErrorMessageConstant.ERROR_UPDATE_PERMISSION);
                    return new CommandResult<NewsProviderViewModel>
                    {
                        isValid = false,
                        errorMessage = ErrorMessageConstant.ERROR_UPDATE_PERMISSION
                    };
                }
            }
            catch (Exception ex)
            {
                await Logging<ApproveNewsProviderServiceCommand>.
                        ErrorAsync(ex, ActionCommand.COMMAND_APPROVE, userName, "Has error");
                return new CommandResult<NewsProviderViewModel>
                {
                    isValid = false,
                    errorMessage = ex.InnerException.ToString()
                };
            }
        }

        private ProviderNew MappingNewProvider(ProviderNew proNew)
        {
            proNew.Status = Status.WaitingApprove;
            return proNew;
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