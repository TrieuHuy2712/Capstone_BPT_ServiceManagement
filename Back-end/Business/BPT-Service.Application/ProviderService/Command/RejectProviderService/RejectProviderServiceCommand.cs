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
using SendGrid;
using SendGrid.Helpers.Mail;

namespace BPT_Service.Application.ProviderService.Command.RejectProviderService
{
    public class RejectProviderServiceCommand : IRejectProviderServiceCommand
    {
        private readonly IRepository<Provider, Guid> _providerRepository;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public RejectProviderServiceCommand(IHttpContextAccessor httpContextAccessor,
        IRepository<Provider, Guid> providerRepository)
        {
            _httpContextAccessor = httpContextAccessor;
            _providerRepository = providerRepository;
        }

        public async Task<CommandResult<ProviderServiceViewModel>> ExecuteAsync(ProviderServiceViewModel vm)
        {
            try
            {
                 var userId = _httpContextAccessor.HttpContext.User.Identity.Name;
                if (userId == null || userId != vm.UserName)
                {
                    return new CommandResult<ProviderServiceViewModel>
                    {
                        isValid = false,
                        myModel = vm
                    };
                }
                var mappingProvider = await _providerRepository.FindByIdAsync(vm.Id);
                if (mappingProvider != null)
                {
                    return new CommandResult<ProviderServiceViewModel>
                    {
                        isValid = false,
                        myModel = vm
                    };
                }
                var map = MappingProvider(mappingProvider, vm);
                _providerRepository.Update(map);
                await _providerRepository.SaveAsync();
                //Set content for email
                var content = "Your provider: " + vm.ProviderName + " has been rejected. Because it is not suitable with my policu";
                ContentEmail(KeySetting.SENDGRIDKEY, ApproveProviderEmailSetting.Subject,
                                content, mappingProvider.AppUser.Email).Wait();
                return new CommandResult<ProviderServiceViewModel>
                {
                    isValid = true,
                    myModel = vm
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

        private Provider MappingProvider(Provider pro, ProviderServiceViewModel vm)
        {
            pro.Id = vm.Id;
            pro.PhoneNumber = vm.PhoneNumber;
            pro.Status = Status.InActive;
            pro.CityId = vm.CityId;
            pro.UserId = vm.UserId;
            pro.TaxCode = vm.TaxCode;
            pro.Description = vm.Description;
            pro.DateModified = DateTime.Now;
            pro.ProviderName = vm.ProviderName;
            pro.Address = vm.Address;
            return pro;
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