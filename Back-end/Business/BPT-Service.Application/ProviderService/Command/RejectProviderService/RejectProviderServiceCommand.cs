using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using BPT_Service.Application.EmailService.Query.GetAllEmailService;
using BPT_Service.Application.ProviderService.Query.CheckUserIsProvider;
using BPT_Service.Application.ProviderService.ViewModel;
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

namespace BPT_Service.Application.ProviderService.Command.RejectProviderService
{
    public class RejectProviderServiceCommand : IRejectProviderServiceCommand
    {
        private readonly IRepository<Provider, Guid> _providerRepository;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly UserManager<AppUser> _userRepository;
        private readonly ICheckUserIsProviderQuery _checkUserIsProviderQuery;
        private readonly IGetAllEmailServiceQuery _getAllEmailServiceQuery;
        private readonly IOptions<EmailConfigModel> _configEmail;

        public RejectProviderServiceCommand(IHttpContextAccessor httpContextAccessor,
        IRepository<Provider, Guid> providerRepository,
        UserManager<AppUser> userRepository,
        ICheckUserIsProviderQuery checkUserIsProviderQuery,
        IOptions<EmailConfigModel> configEmail,
        IGetAllEmailServiceQuery getAllEmailServiceQuery)
        {
            _httpContextAccessor = httpContextAccessor;
            _providerRepository = providerRepository;
            _userRepository = userRepository;
            _checkUserIsProviderQuery = checkUserIsProviderQuery;
            _configEmail = configEmail;
            _getAllEmailServiceQuery = getAllEmailServiceQuery;
        }

        public async Task<CommandResult<ProviderServiceViewModel>> ExecuteAsync(string providerId, string reason)
        {
            try
            {
                var userId = _httpContextAccessor.HttpContext.User.Identity.Name;
                if (userId == null)
                {
                    return new CommandResult<ProviderServiceViewModel>
                    {
                        isValid = false,
                        myModel = null
                    };
                }
                if (_checkUserIsProviderQuery.ExecuteAsync().Result.isValid == true)
                {
                    var getUser = await _userRepository.FindByIdAsync(userId);
                    var getAllRole = await _userRepository.GetRolesAsync(getUser);
                    var providerCheck = await _userRepository.IsInRoleAsync(getUser, "PROVIDER"); 
                    var check= await _userRepository.RemoveFromRoleAsync(getUser, "Provider");
                }
                var mappingProvider = await _providerRepository.FindByIdAsync(Guid.Parse(providerId));
                if (mappingProvider == null)
                {
                    return new CommandResult<ProviderServiceViewModel>
                    {
                        isValid = false,
                        myModel = null
                    };
                }
                mappingProvider.Status = Status.InActive;
                _providerRepository.Update(mappingProvider);
                await _providerRepository.SaveAsync();

                var getEmail = await _userRepository.FindByIdAsync(mappingProvider.UserId.ToString());
                //Set content for email
                var getEmailContent = await _getAllEmailServiceQuery.ExecuteAsync();
                var getFirstEmail = getEmailContent.Where(x => x.Name == EmailName.Reject_Provider).FirstOrDefault();
                getFirstEmail.Message= getFirstEmail.Message.Replace(EmailKey.UserNameKey, getEmail.Email).Replace(EmailKey.ReasonKey, reason);
                ContentEmail(_configEmail.Value.SendGridKey, getFirstEmail.Subject,
                                getFirstEmail.Message, mappingProvider.AppUser.Email).Wait();
                return new CommandResult<ProviderServiceViewModel>
                {
                    isValid = true,
                    myModel = null,
                };
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