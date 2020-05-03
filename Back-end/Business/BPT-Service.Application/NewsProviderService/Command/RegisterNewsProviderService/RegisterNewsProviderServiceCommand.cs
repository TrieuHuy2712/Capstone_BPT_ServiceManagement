using BPT_Service.Application.EmailService.Query.GetAllEmailService;
using BPT_Service.Application.NewsProviderService.ViewModel;
using BPT_Service.Application.PermissionService.Query.CheckUserIsAdmin;
using BPT_Service.Application.PermissionService.Query.GetPermissionAction;
using BPT_Service.Application.ProviderService.Query.CheckUserIsProvider;
using BPT_Service.Common;
using BPT_Service.Common.Constants;
using BPT_Service.Common.Constants.EmailConstant;
using BPT_Service.Common.Dtos;
using BPT_Service.Common.Helpers;
using BPT_Service.Common.Logging;
using BPT_Service.Common.Support;
using BPT_Service.Model.Entities;
using BPT_Service.Model.Entities.ServiceModel;
using BPT_Service.Model.Entities.ServiceModel.ProviderServiceModel;
using BPT_Service.Model.Enums;
using BPT_Service.Model.Infrastructure.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using SendGrid;
using SendGrid.Helpers.Mail;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace BPT_Service.Application.NewsProviderService.Command.RegisterNewsProviderService
{
    public class RegisterNewsProviderServiceCommand : IRegisterNewsProviderServiceCommand
    {
        private readonly ICheckUserIsAdminQuery _checkUserIsAdminQuery;
        private readonly ICheckUserIsProviderQuery _checkUserIsProviderQuery;
        private readonly IConfiguration _configuration;
        private readonly IGetAllEmailServiceQuery _getAllEmailServiceQuery;
        private readonly IGetPermissionActionQuery _getPermissionActionQuery;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IOptions<EmailConfigModel> _config;
        private readonly IRepository<Provider, Guid> _providerRepository;
        private readonly IRepository<ProviderNew, int> _newProviderRepository;
        private readonly UserManager<AppUser> _userManager;

        public RegisterNewsProviderServiceCommand(
            ICheckUserIsAdminQuery checkUserIsAdminQuery, 
            ICheckUserIsProviderQuery checkUserIsProviderQuery, 
            IConfiguration configuration, 
            IGetAllEmailServiceQuery getAllEmailServiceQuery, 
            IGetPermissionActionQuery getPermissionActionQuery, 
            IHttpContextAccessor httpContextAccessor, 
            IOptions<EmailConfigModel> config, 
            IRepository<Provider, Guid> providerRepository, 
            IRepository<ProviderNew, int> newProviderRepository, 
            UserManager<AppUser> userManager)
        {
            _checkUserIsAdminQuery = checkUserIsAdminQuery;
            _checkUserIsProviderQuery = checkUserIsProviderQuery;
            _configuration = configuration;
            _getAllEmailServiceQuery = getAllEmailServiceQuery;
            _getPermissionActionQuery = getPermissionActionQuery;
            _httpContextAccessor = httpContextAccessor;
            _config = config;
            _providerRepository = providerRepository;
            _newProviderRepository = newProviderRepository;
            _userManager = userManager;
        }

        public async Task<CommandResult<NewsProviderViewModel>> ExecuteAsync(NewsProviderViewModel vm)
        {
            var userId = _httpContextAccessor.HttpContext.User.Identity.Name;
            var userName = _userManager.FindByIdAsync(userId).Result.UserName;
            try
            {
                var getIsProvider = await _checkUserIsProviderQuery.ExecuteAsync(userId);
                if (await _checkUserIsAdminQuery.ExecuteAsync(userId) ||
                   await _getPermissionActionQuery.ExecuteAsync(userId, ConstantFunctions.NEWS, ActionSetting.CanCreate) ||
                   getIsProvider.isValid)
                {
                    var mappingProvider = await MappingProvider(vm, Guid.Parse(vm.ProviderId), userId);
                    mappingProvider.CodeConfirm = RandomCodeSupport.RandomString(6);
                    await _newProviderRepository.Add(mappingProvider);
                    await _newProviderRepository.SaveAsync();
                    vm.Id = mappingProvider.Id;
                    vm.Status = mappingProvider.Status;
                    await Logging<RegisterNewsProviderServiceCommand>.
                        InformationAsync(ActionCommand.COMMAND_ADD, userName, JsonConvert.SerializeObject(vm));
                    //Send mail confirm

                    //Get Provider Information
                    var providerInformation = await _providerRepository.FindByIdAsync(Guid.Parse(vm.ProviderId));
                    var findUser = await _userManager.FindByIdAsync(providerInformation.UserId.ToString());
                    //Set content for email
                    var getEmailContent = await _getAllEmailServiceQuery.ExecuteAsync();
                    var generateCode = _configuration.GetSection("Host").GetSection("LinkConfirmNewsProvider").Value +
                         mappingProvider.CodeConfirm + '_' + mappingProvider.Id;

                    var getFirstEmail = getEmailContent.Where(x => x.Name == EmailName.Approve_News).FirstOrDefault();
                    getFirstEmail.Message = getFirstEmail.Message.Replace(EmailKey.UserNameKey, findUser.Email).
                        Replace(EmailKey.ConfirmLink, generateCode);

                    ContentEmail(_config.Value.SendGridKey, getFirstEmail.Subject,
                                    getFirstEmail.Message, findUser.Email).Wait();
                    return new CommandResult<NewsProviderViewModel>
                    {
                        isValid = true,
                        myModel = vm
                    };
                }
                else
                {
                    await Logging<RegisterNewsProviderServiceCommand>.
                           WarningAsync(ActionCommand.COMMAND_ADD, userName, ErrorMessageConstant.ERROR_DELETE_PERMISSION);
                    return new CommandResult<NewsProviderViewModel>
                    {
                        isValid = true,
                        errorMessage = ErrorMessageConstant.ERROR_ADD_PERMISSION
                    };
                }
            }
            catch (System.Exception ex)
            {
                await Logging<RegisterNewsProviderServiceCommand>.
                       ErrorAsync(ex, ActionCommand.COMMAND_ADD, userName, "Has error");
                return new CommandResult<NewsProviderViewModel>
                {
                    isValid = false,
                    myModel = vm,
                    errorMessage = ex.InnerException.ToString()
                };
            }
        }

        private async Task<ProviderNew> MappingProvider(NewsProviderViewModel vm, Guid providerId, string currentUserContext)
        {
            ProviderNew pro = new ProviderNew();
            pro.Author = vm.Author;
            pro.Status = (await _getPermissionActionQuery.ExecuteAsync(currentUserContext, ConstantFunctions.NEWS, ActionSetting.CanCreate)
                || await _checkUserIsAdminQuery.ExecuteAsync(currentUserContext)) ? Status.WaitingApprove : Status.Pending;
            pro.Author = vm.Author;
            pro.ProviderId = providerId;
            pro.Title = vm.Title;
            pro.Content = vm.Content;
            pro.DateCreated = DateTime.Now;
            pro.ImgPath = vm.ImgPath;
            return pro;
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