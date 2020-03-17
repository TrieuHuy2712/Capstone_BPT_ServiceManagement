using System;
using System.Threading.Tasks;
using BPT_Service.Application.PermissionService.Query.GetPermissionAction;
using BPT_Service.Application.PostService.ViewModel;
using BPT_Service.Common.Helpers;
using BPT_Service.Model.Entities;
using BPT_Service.Model.Entities.ServiceModel;
using BPT_Service.Model.Enums;
using BPT_Service.Model.Infrastructure.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using SendGrid;
using SendGrid.Helpers.Mail;

namespace BPT_Service.Application.PostService.Command.ApprovePostService
{
    public class ApprovePostServiceCommand : IApprovePostServiceCommand
    {
        private readonly IRepository<Service, Guid> _postServiceRepository;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly UserManager<AppUser> _userRepository;
        private readonly IRepository<Model.Entities.ServiceModel.UserServiceModel.UserService, int> _userServiceRepository;
        private readonly IRepository<Model.Entities.ServiceModel.ProviderServiceModel.ProviderService, int> _providerServiceRepository;
        private readonly IRepository<Provider, Guid> _providerRepository;
        private readonly IGetPermissionActionQuery _getPermissionActionQuery;

        public ApprovePostServiceCommand(IRepository<Service, Guid> postServiceRepository,
            IHttpContextAccessor httpContextAccessor,
             UserManager<AppUser> userRepository,
             IRepository<BPT_Service.Model.Entities.ServiceModel.UserServiceModel.UserService, int> userServiceRepository,
             IRepository<BPT_Service.Model.Entities.ServiceModel.ProviderServiceModel.ProviderService, int> providerServiceRepository,
             IRepository<Provider, Guid> providerRepository,
             IGetPermissionActionQuery getPermissionActionQuery)
        {
            _postServiceRepository = postServiceRepository;
            _httpContextAccessor = httpContextAccessor;
            _userRepository = userRepository;
            _userServiceRepository = userServiceRepository;
            _providerServiceRepository = providerServiceRepository;
            _providerRepository = providerRepository;
            _getPermissionActionQuery = getPermissionActionQuery;
        }
        public async Task<CommandResult<PostServiceViewModel>> ExecuteAsync(string idService)
        {
            try
            {
                var userId = _httpContextAccessor.HttpContext.User.Identity.Name;
                if(!await _getPermissionActionQuery.ExecuteAsync(userId,"Service", ActionSetting.CanUpdate))
                {
                    return new CommandResult<PostServiceViewModel>
                    {
                        isValid = false,
                        errorMessage = "You don't have permission"
                    };
                }
                var idUser = "";
                var getCurrentPost = await _postServiceRepository.FindByIdAsync(Guid.Parse(idService));
                if (getCurrentPost != null)
                {
                    getCurrentPost.Status = Status.Active;
                    _postServiceRepository.Update(getCurrentPost);
                    await _postServiceRepository.SaveAsync();

                    //Check is UserService
                    var serviceInformation = await  _userServiceRepository.FindSingleAsync(x => x.ServiceId == Guid.Parse(idService));
                    if(serviceInformation == null)
                    {
                        //Check is ProviderService
                        var providerInformation = await _providerServiceRepository.FindSingleAsync(x => x.ServiceId == Guid.Parse(idService));
                        if(providerInformation != null)
                        {
                            var getProviderInformation = await _providerRepository.FindSingleAsync(x => x.Id == providerInformation.ProviderId);
                            idUser = getProviderInformation.UserId.ToString();
                        }
                        else
                        {
                            return new CommandResult<PostServiceViewModel>
                            {
                                isValid = false,
                            };
                        }
                    }
                    else
                    {
                        idUser = serviceInformation.UserId.ToString();
                    }

                    if(idUser != "")
                    {
                        var findEmailUser = await _userRepository.FindByIdAsync(idUser);
                        if (findEmailUser != null)
                        {
                            //Set content for email
                            var content = "Your provider: " + getCurrentPost.ServiceName + " has been approved. Please check in our system";
                            ContentEmail(KeySetting.SENDGRIDKEY, ApproveProviderEmailSetting.Subject,
                                            content, findEmailUser.Email).Wait();
                        }
                        else
                        {
                            return new CommandResult<PostServiceViewModel>
                            {
                                isValid = false,
                            };
                        }
                    }
                   
                    return new CommandResult<PostServiceViewModel>
                    {
                        isValid = true,
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
            var from = new EmailAddress(ApproveServiceEmailSetting.FromUserEmail, ApproveServiceEmailSetting.FullNameUser);
            var subject = subject1;
            var to = new EmailAddress(email);
            var plainTextContent = message;
            var htmlContent = "<strong>" + message + "</strong>";
            var msg = MailHelper.CreateSingleEmail(from, to, subject, plainTextContent, htmlContent);
            var response = await client.SendEmailAsync(msg);
        }
    }
}