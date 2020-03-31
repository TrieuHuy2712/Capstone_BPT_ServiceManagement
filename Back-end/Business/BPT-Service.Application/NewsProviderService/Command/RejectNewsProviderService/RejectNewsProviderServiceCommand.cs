using BPT_Service.Application.EmailService.Query.GetAllEmailService;
using BPT_Service.Application.NewsProviderService.ViewModel;
using BPT_Service.Application.PermissionService.Query.CheckUserIsAdmin;
using BPT_Service.Application.PermissionService.Query.GetPermissionAction;
using BPT_Service.Application.ProviderService.Query.CheckUserIsProvider;
using BPT_Service.Common;
using BPT_Service.Common.Constants.EmailConstant;
using BPT_Service.Common.Dtos;
using BPT_Service.Common.Helpers;
using BPT_Service.Model.Entities;
using BPT_Service.Model.Entities.ServiceModel;
using BPT_Service.Model.Entities.ServiceModel.ProviderServiceModel;
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

namespace BPT_Service.Application.NewsProviderService.Command.RejectNewsProvider
{
    public class RejectNewsProviderServiceCommand : IRejectNewsProviderServiceCommand
    {
        private readonly IRepository<ProviderNew, int> _newProviderRepository;
        private readonly UserManager<AppUser> _userRepository;
        private readonly IRepository<Provider, Guid> _providerRepository;
        private readonly IGetAllEmailServiceQuery _getAllEmailServiceQuery;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IOptions<EmailConfigModel> _configEmail;
        private readonly IGetPermissionActionQuery _getPermissionActionQuery;
        private readonly ICheckUserIsAdminQuery _checkUserIsAdminQuery;

        public RejectNewsProviderServiceCommand(IRepository<ProviderNew, int> newProviderRepository,
        IHttpContextAccessor httpContextAccessor,
        UserManager<AppUser> userRepository,
        IRepository<Provider, Guid> providerRepository,
        IGetAllEmailServiceQuery getAllEmailServiceQuery,
        IOptions<EmailConfigModel> configEmail,
        IGetPermissionActionQuery getPermissionActionQuery,
        ICheckUserIsAdminQuery checkUserIsAdminQuery)
        {
            _checkUserIsAdminQuery = checkUserIsAdminQuery;
            _configEmail = configEmail;
            _getAllEmailServiceQuery = getAllEmailServiceQuery;
            _getPermissionActionQuery = getPermissionActionQuery;
            _httpContextAccessor = httpContextAccessor;
            _newProviderRepository = newProviderRepository;
            _providerRepository = providerRepository;
            _userRepository = userRepository;
        }

        public async Task<CommandResult<NewsProviderViewModel>> ExecuteAsync(int id, string reason)
        {
            try
            {
                var userId = _httpContextAccessor.HttpContext.User.Identity.Name;
                if (await _checkUserIsAdminQuery.ExecuteAsync(userId) || await _getPermissionActionQuery.ExecuteAsync(userId, "NEWS", ActionSetting.CanUpdate))
                {
                    var mappingProvider = await _newProviderRepository.FindByIdAsync(id);
                    if (mappingProvider != null)
                    {
                        return new CommandResult<NewsProviderViewModel>
                        {
                            isValid = false,
                            errorMessage = "Cannot find your new"
                        };
                    }
                    var map = MappingNewProvider(mappingProvider);
                    _newProviderRepository.Update(map);
                    await _newProviderRepository.SaveAsync();

                    //Send mail service
                    var getProvider = await _providerRepository.FindSingleAsync(x => x.Id == mappingProvider.ProviderId);
                    var getEmail = await _userRepository.FindByIdAsync(getProvider.UserId.ToString());

                    //Set content for email
                    var getAllEmail = await _getAllEmailServiceQuery.ExecuteAsync();
                    var getFirstEmail = getAllEmail.Where(x => x.Name == EmailName.Reject_News).FirstOrDefault();
                    getFirstEmail.Message = getFirstEmail.Message.Replace(EmailKey.UserNameKey, getEmail.UserName)
                                                .Replace(EmailKey.NewNameKey, mappingProvider.Title)
                                                .Replace(EmailKey.ReasonKey, reason);

                    ContentEmail(_configEmail.Value.SendGridKey, getFirstEmail.Subject,
                                    getFirstEmail.Message, getEmail.Email).Wait();
                    return new CommandResult<NewsProviderViewModel>
                    {
                        isValid = true,
                        errorMessage = "You have been accepted. Please check your email"
                    };
                }
                else
                {
                    return new CommandResult<NewsProviderViewModel>
                    {
                        isValid = false,
                        errorMessage = ErrorMessageConstant.ERROR_UPDATE_PERMISSION
                    };
                }
            }
            catch (Exception ex)
            {
                return new CommandResult<NewsProviderViewModel>
                {
                    isValid = false,
                    errorMessage = ex.InnerException.ToString()
                };
            }
        }

        private ProviderNew MappingNewProvider(ProviderNew proNew)
        {
            proNew.Status = Status.InActive;
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