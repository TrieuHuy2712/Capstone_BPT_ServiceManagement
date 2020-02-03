using System;
using System.Threading.Tasks;
using BPT_Service.Application.NewsProviderService.ViewModel;
using BPT_Service.Application.ProviderService.ViewModel;
using BPT_Service.Common.Helpers;
using BPT_Service.Common.Helpers.NewsProviderEmail;
using BPT_Service.Model.Entities;
using BPT_Service.Model.Entities.ServiceModel.ProviderServiceModel;
using BPT_Service.Model.Enums;
using BPT_Service.Model.Infrastructure.Interfaces;
using Microsoft.AspNetCore.Http;
using SendGrid;
using SendGrid.Helpers.Mail;

namespace BPT_Service.Application.NewsProviderService.Command.RejectNewsProvider
{
    public class RejectNewsProviderService : IRejectNewsProviderService
    {
        private readonly IRepository<ProviderNew, int> _newProviderRepository;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public RejectNewsProviderService(IRepository<ProviderNew, int> newProviderRepository, IHttpContextAccessor httpContextAccessor)
        {
            _newProviderRepository = newProviderRepository;
            _httpContextAccessor = httpContextAccessor;
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
                var map = MappingNewProvider(mappingProvider, vm);
                _newProviderRepository.Update(map);
                await _newProviderRepository.SaveAsync();

                //Set content for email
                var content = "Your news: " + vm.Title + " has been rejected. Please check in our system";
                ContentEmail(KeySetting.SENDGRIDKEY, RejectNewsProviderEmailSetting.Subject,
                                content, mappingProvider.Provider.AppUser.Email).Wait();
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
        private ProviderNew MappingNewProvider(ProviderNew proNew, NewsProviderViewModel vm)
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