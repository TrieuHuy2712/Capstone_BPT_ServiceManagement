using BPT_Service.Application.PostService.ViewModel;
using BPT_Service.Common.Helpers;
using BPT_Service.Model.Entities.ServiceModel;
using BPT_Service.Model.Entities;
using BPT_Service.Model.Enums;
using BPT_Service.Model.Infrastructure.Interfaces;
using Microsoft.AspNetCore.Http;
using SendGrid.Helpers.Mail;
using SendGrid;
using System.Threading.Tasks;
using System;

namespace BPT_Service.Application.PostService.Command.RejectPostService
{
    public class RejectPostServiceCommand : IRejectPostServiceCommand
    {
        private readonly IRepository<Service, Guid> _postServiceRepository;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public RejectPostServiceCommand(IRepository<Service, Guid> postServiceRepository, IHttpContextAccessor httpContextAccessor)
        {
            _postServiceRepository = postServiceRepository;
            _httpContextAccessor = httpContextAccessor;
        }
        public async Task<CommandResult<PostServiceViewModel>> ExecuteAsync(PostServiceViewModel vm)
        {
            try
            {
                var getCurrentPost = await _postServiceRepository.FindByIdAsync(vm.Id);
                if (getCurrentPost != null)
                {
                    getCurrentPost.Status = Status.InActive;
                    _postServiceRepository.Update(getCurrentPost);
                    await _postServiceRepository.SaveAsync();

                    //Set content for email
                    var content = "Your provider: " + vm.ServiceName + " has been rejected. " + vm.Reason;
                    ContentEmail(KeySetting.SENDGRIDKEY, RejectServiceEmailSetting.Subject,
                                    content, vm.Email).Wait();
                    return new CommandResult<PostServiceViewModel>
                    {
                        isValid = true,
                        myModel = vm
                    };
                }
                return new CommandResult<PostServiceViewModel>
                {
                    isValid = false,
                    errorMessage = "Cannot find Id of PostService"
                };
            }
            catch (System.Exception ex)
            {
                return new CommandResult<PostServiceViewModel>
                {
                    isValid = false,
                    errorMessage = ex.InnerException.ToString()
                };
            }
        }

        private async Task ContentEmail(string apiKey, string subject1, string message, string email)
        {
            var client = new SendGridClient(apiKey);
            var from = new EmailAddress(RejectServiceEmailSetting.FromUserEmail, RejectServiceEmailSetting.FullNameUser);
            var subject = subject1;
            var to = new EmailAddress(email);
            var plainTextContent = message;
            var htmlContent = "<strong>" + message + "</strong>";
            var msg = MailHelper.CreateSingleEmail(from, to, subject, plainTextContent, htmlContent);
            var response = await client.SendEmailAsync(msg);
        }
    }
}