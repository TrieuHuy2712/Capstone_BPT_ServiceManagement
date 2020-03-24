using System;
using System.Security.Claims;
using System.Threading.Tasks;
using BPT_Service.Application.ProviderService.Query.CheckUserIsProvider;
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

namespace BPT_Service.Application.ProviderService.Command.RejectProviderService
{
    public class RejectProviderServiceCommand : IRejectProviderServiceCommand
    {
        private readonly IRepository<Provider, Guid> _providerRepository;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly UserManager<AppUser> _userRepository;
        private readonly ICheckUserIsProviderQuery _checkUserIsProviderQuery;

        public RejectProviderServiceCommand(IHttpContextAccessor httpContextAccessor,
        IRepository<Provider, Guid> providerRepository,
        UserManager<AppUser> userRepository,
        ICheckUserIsProviderQuery checkUserIsProviderQuery)
        {
            _httpContextAccessor = httpContextAccessor;
            _providerRepository = providerRepository;
            _userRepository = userRepository;
            _checkUserIsProviderQuery = checkUserIsProviderQuery;
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
                var content = "Your provider: " + getEmail.Email + " has been rejected. Because it is not suitable with my policy. "+ reason;
                ContentEmail(KeySetting.SENDGRIDKEY, ApproveProviderEmailSetting.Subject,
                                content, mappingProvider.AppUser.Email).Wait();
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
            var from = new EmailAddress(RejectProviderEmailSetting.FromUserEmail, ApproveProviderEmailSetting.FullNameUser);
            var subject = subject1;
            var to = new EmailAddress(email);
            var plainTextContent = message;
            var htmlContent = "<strong>" + message + "</strong>";
            var msg = MailHelper.CreateSingleEmail(from, to, subject, plainTextContent, htmlContent);
            var response = await client.SendEmailAsync(msg);
        }
    }
}