using BPT_Service.Application.EmailService.Query.GetAllEmailService;
using BPT_Service.Application.NewsProviderService.ViewModel;
using BPT_Service.Application.PermissionService.Query.CheckUserIsAdmin;
using BPT_Service.Application.PermissionService.Query.GetPermissionAction;
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

        public ApproveNewsProviderServiceCommand(
            IGetAllEmailServiceQuery getAllEmailServiceQuery,
            IHttpContextAccessor httpContextAccessor,
            IOptions<EmailConfigModel> configEmail,
            IRepository<Provider, Guid> providerRepository,
            IRepository<ProviderNew, int> newProviderRepository,
            UserManager<AppUser> userRepository,
            ICheckUserIsAdminQuery checkUserIsAdminQuery,
            IGetPermissionActionQuery getPermissionActionQuery)
        {
            _configEmail = configEmail;
            _getAllEmailServiceQuery = getAllEmailServiceQuery;
            _httpContextAccessor = httpContextAccessor;
            _newProviderRepository = newProviderRepository;
            _providerRepository = providerRepository;
            _userRepository = userRepository;
            _checkUserIsAdminQuery = checkUserIsAdminQuery;
            _getPermissionActionQuery = getPermissionActionQuery;
        }

        public async Task<CommandResult<NewsProviderViewModel>> ExecuteAsync(int idNews)
        {
            try
            {//Check user has permission first
                var userId = _httpContextAccessor.HttpContext.User.Identity.Name;
                if (await _checkUserIsAdminQuery.ExecuteAsync(userId) || await _getPermissionActionQuery.ExecuteAsync(userId, "NEWS", ActionSetting.CanUpdate))
                {
                    var mappingProvider = await _newProviderRepository.FindByIdAsync(idNews);
                    if (mappingProvider != null)
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
                    getFirstEmail.Message = getFirstEmail.Message.Replace(EmailKey.UserNameKey, getEmail.UserName).Replace(EmailKey.NewNameKey, mappingProvider.Title);
                    ContentEmail(_configEmail.Value.SendGridKey, getFirstEmail.Subject,
                                    getFirstEmail.Message, getEmail.Email).Wait();
                    return new CommandResult<NewsProviderViewModel>
                    {
                        isValid = true,
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
            proNew.Status = Status.Active;
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