using System;
using System.Threading.Tasks;
using BPT_Service.Application.NewsProviderService.ViewModel;
using BPT_Service.Application.ProviderService.ViewModel;
using BPT_Service.Common.Helpers;
using BPT_Service.Common.Helpers.NewsProviderEmail;
using BPT_Service.Model.Entities;
using BPT_Service.Model.Entities.ServiceModel;
using BPT_Service.Model.Entities.ServiceModel.ProviderServiceModel;
using BPT_Service.Model.Enums;
using BPT_Service.Model.Infrastructure.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using SendGrid;
using SendGrid.Helpers.Mail;

namespace BPT_Service.Application.NewsProviderService.Command.RejectNewsProvider
{
    public class RejectNewsProviderServiceCommand : IRejectNewsProviderServiceCommand
    {
        private readonly IRepository<ProviderNew, int> _newProviderRepository;
        private readonly UserManager<AppUser> _userRepository;
        private readonly IRepository<Provider, Guid> _providerRepository;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public RejectNewsProviderServiceCommand(IRepository<ProviderNew, int> newProviderRepository,
        IHttpContextAccessor httpContextAccessor,
        UserManager<AppUser> userRepository,
        IRepository<Provider, Guid> providerRepository)
        {
            _newProviderRepository = newProviderRepository;
            _httpContextAccessor = httpContextAccessor;
            _userRepository = userRepository;
            _providerRepository = providerRepository;
        }

        public async Task<CommandResult<NewsProviderViewModel>> ExecuteAsync(NewsProviderViewModel vm)
        {
            try
            {
                var userId = _httpContextAccessor.HttpContext.User.Identity.Name;
                if (userId == null || userId != vm.UserId)
                {
                    return new CommandResult<NewsProviderViewModel>
                    {
                        isValid = false,
                        myModel = vm
                    };
                }
                var mappingProvider = await _newProviderRepository.FindByIdAsync(vm.Id);
                if (mappingProvider != null)
                {
                    return new CommandResult<NewsProviderViewModel>
                    {
                        isValid = false,
                        myModel = vm
                    };
                }
                var map = MappingNewProvider(mappingProvider);
                _newProviderRepository.Update(map);
                await _newProviderRepository.SaveAsync();

                var getProvider = await _providerRepository.FindSingleAsync(x => x.Id == mappingProvider.ProviderId);
                var getEmail = await _userRepository.FindByIdAsync(getProvider.UserId.ToString());

                //Set content for email
                var content = "Your news: " + mappingProvider.Title + " has been rejected. Please check in our system. Because " + vm.Reason;
                ContentEmail(KeySetting.SENDGRIDKEY, RejectNewsProviderEmailSetting.Subject,
                                content, getEmail.Email).Wait();
                return new CommandResult<NewsProviderViewModel>
                {
                    isValid = true,
                    myModel = vm
                };
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
            var from = new EmailAddress(RejectNewsProviderEmailSetting.FromUserEmail, RejectNewsProviderEmailSetting.FullNameUser);
            var subject = subject1;
            var to = new EmailAddress(email);
            var plainTextContent = message;
            var htmlContent = "<strong>" + message + "</strong>";
            var msg = MailHelper.CreateSingleEmail(from, to, subject, plainTextContent, htmlContent);
            var response = await client.SendEmailAsync(msg);
        }
    }
}