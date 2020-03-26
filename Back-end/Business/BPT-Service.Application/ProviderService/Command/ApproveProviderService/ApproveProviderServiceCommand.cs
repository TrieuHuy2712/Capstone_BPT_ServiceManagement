using BPT_Service.Application.EmailService.Query.GetAllEmailService;
using BPT_Service.Application.PostService.Command.PostServiceFromUser.DeleteServiceFromUser;
using BPT_Service.Application.PostService.Query.GetPostUserServiceByUserId;
using BPT_Service.Application.ProviderService.Query.CheckUserIsProvider;
using BPT_Service.Application.ProviderService.ViewModel;
using BPT_Service.Common.Constants.EmailConstant;
using BPT_Service.Common.Dtos;
using BPT_Service.Model.Entities;
using BPT_Service.Model.Entities.ServiceModel;
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

namespace BPT_Service.Application.ProviderService.Command.ApproveProviderService
{
    public class ApproveProviderServiceCommand : IApproveProviderServiceCommand
    {
        private readonly IRepository<Provider, Guid> _providerRepository;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly UserManager<AppUser> _userRepository;
        private readonly ICheckUserIsProviderQuery _checkUserIsProviderQuery;
        private readonly IGetAllEmailServiceQuery _getAllEmailServiceQuery;
        private readonly IOptions<EmailConfigModel> _config;
        private readonly IGetPostUserServiceByUserIdQuery _getPostUserServiceByUserIdQuery;
        private readonly IDeleteServiceFromUserCommand _deleteServiceFromUserCommand;

        public ApproveProviderServiceCommand(
            IHttpContextAccessor httpContextAccessor,
            IRepository<Provider, Guid> providerRepository,
            UserManager<AppUser> userRepository,
            ICheckUserIsProviderQuery checkUserIsProviderQuery,
            IGetAllEmailServiceQuery getAllEmailServiceQuery,
            IOptions<EmailConfigModel> config,
            IGetPostUserServiceByUserIdQuery getPostUserServiceByUserIdQuery,
            IDeleteServiceFromUserCommand deleteServiceFromUserCommand)
        {
            _httpContextAccessor = httpContextAccessor;
            _providerRepository = providerRepository;
            _userRepository = userRepository;
            _checkUserIsProviderQuery = checkUserIsProviderQuery;
            _getAllEmailServiceQuery = getAllEmailServiceQuery;
            _config = config;
            _getPostUserServiceByUserIdQuery = getPostUserServiceByUserIdQuery;
            _deleteServiceFromUserCommand = deleteServiceFromUserCommand;
        }

        public async Task<CommandResult<ProviderServiceViewModel>> ExecuteAsync(string userProvider, string providerId)
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
                var getUserService = await _getPostUserServiceByUserIdQuery.ExecuteAsync(userProvider);
                if (getUserService != null)
                {
                    await _deleteServiceFromUserCommand.ExecuteAsync(getUserService.Id);
                }

                var mappingProvider = await _providerRepository.FindByIdAsync(Guid.Parse(providerId));
                if (mappingProvider == null)
                {
                    return new CommandResult<ProviderServiceViewModel>
                    {
                        isValid = false,
                        errorMessage = "Cannot find your id provider"
                    };
                }
                //Check user is Provider
                if (_checkUserIsProviderQuery.ExecuteAsync().Result.isValid == true)
                {
                    return new CommandResult<ProviderServiceViewModel>
                    {
                        errorMessage = "You had been a provider"
                    };
                }
                mappingProvider.Status = Status.Active;
                _providerRepository.Update(mappingProvider);
                var findUserId = await _userRepository.FindByIdAsync(userId);
                await _userRepository.AddToRoleAsync(findUserId, "Provider");
                await _providerRepository.SaveAsync();
                var userMail = await _userRepository.FindByIdAsync(mappingProvider.UserId.ToString());

                //Set content for email
                //var content = "Your provider: " + userMail.Email + " has been approved. Please check in our system";
                var getEmailContent = await _getAllEmailServiceQuery.ExecuteAsync();
                var getFirstEmail = getEmailContent.Where(x => x.Name == EmailName.Approve_Provider).FirstOrDefault();
                getFirstEmail.Message = getFirstEmail.Message.Replace(EmailKey.UserNameKey, userMail.Email);

                ContentEmail(_config.Value.SendGridKey, getFirstEmail.Subject,
                                getFirstEmail.Message, _userRepository.FindByIdAsync(userId).Result.Email).Wait();
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
            var from = new EmailAddress(_config.Value.FromUserEmail, _config.Value.FullUserName);
            var subject = subject1;
            var to = new EmailAddress(email);
            var plainTextContent = message;
            var htmlContent = "<strong>" + message + "</strong>";
            var msg = MailHelper.CreateSingleEmail(from, to, subject, plainTextContent, htmlContent);
            var response = await client.SendEmailAsync(msg);
        }
    }
}