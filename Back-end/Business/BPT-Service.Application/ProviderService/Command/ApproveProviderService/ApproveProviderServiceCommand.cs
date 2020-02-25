using System;
using System.Security.Claims;
using System.Threading.Tasks;
using BPT_Service.Application.ProviderService.ViewModel;
using BPT_Service.Common.Helpers;
using BPT_Service.Model.Entities;
using BPT_Service.Model.Entities.ServiceModel;
using BPT_Service.Model.Enums;
using BPT_Service.Model.Infrastructure.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using SendGrid;
using SendGrid.Helpers.Mail;

namespace BPT_Service.Application.ProviderService.Command.ApproveProviderService
{
    public class ApproveProviderServiceCommand : IApproveProviderServiceCommand
    {
        private readonly IRepository<Provider, Guid> _providerRepository;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly UserManager<AppUser> _userRepository;
        public ApproveProviderServiceCommand(IHttpContextAccessor httpContextAccessor,
        IRepository<Provider, Guid> providerRepository,
        UserManager<AppUser> userRepository)
        {
            _httpContextAccessor = httpContextAccessor;
            _providerRepository = providerRepository;
            _userRepository = userRepository;
        }

        public async Task<CommandResult<ProviderServiceViewModel>> ExecuteAsync(string providerId)
        {
            try
            {
                 var userId = _httpContextAccessor.HttpContext.User.Identity.Name;
                if (userId == null)
                {
                    return new CommandResult<ProviderServiceViewModel>
                    {
                        isValid = false,
                    };
                }
                var mappingProvider = await _providerRepository.FindByIdAsync(Guid.Parse(providerId));
                if (mappingProvider != null)
                {
                    return new CommandResult<ProviderServiceViewModel>
                    {
                        isValid = false,
                    };
                }
                mappingProvider.Status = Status.Active;
                _providerRepository.Update(mappingProvider);
                await _providerRepository.SaveAsync();
                var userMail= await _userRepository.FindByIdAsync(mappingProvider.UserId.ToString());

                //Set content for email
                var content = "Your provider: " + userMail.Email + " has been approved. Please check in our system";
                ContentEmail(KeySetting.SENDGRIDKEY, ApproveProviderEmailSetting.Subject,
                                content, mappingProvider.AppUser.Email).Wait();
                return new CommandResult<ProviderServiceViewModel>
                {
                    isValid = true,
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
            var from = new EmailAddress(ApproveProviderEmailSetting.FromUserEmail, ApproveProviderEmailSetting.FullNameUser);
            var subject = subject1;
            var to = new EmailAddress(email);
            var plainTextContent = message;
            var htmlContent = "<strong>" + message + "</strong>";
            var msg = MailHelper.CreateSingleEmail(from, to, subject, plainTextContent, htmlContent);
            var response = await client.SendEmailAsync(msg);
        }
    }
}