using BPT_Service.Application.EmailService.Query.GetAllEmailService;
using BPT_Service.Application.PermissionService.Query.CheckUserIsAdmin;
using BPT_Service.Application.PermissionService.Query.GetPermissionAction;
using BPT_Service.Application.ProviderService.Query.CheckUserIsProvider;
using BPT_Service.Application.ProviderService.ViewModel;
using BPT_Service.Common.Constants;
using BPT_Service.Common.Constants.EmailConstant;
using BPT_Service.Common.Dtos;
using BPT_Service.Common.Helpers;
using BPT_Service.Common.Logging;
using BPT_Service.Common.Support;
using BPT_Service.Model.Entities;
using BPT_Service.Model.Entities.ServiceModel;
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

namespace BPT_Service.Application.ProviderService.Command.RegisterProviderService
{
    public class RegisterProviderServiceCommand : IRegisterProviderServiceCommand
    {
        private readonly IRepository<Provider, Guid> _providerRepository;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ICheckUserIsAdminQuery _checkUserIsAdminQuery;
        private readonly IGetPermissionActionQuery _getPermissionActionQuery;
        private readonly ICheckUserIsProviderQuery _checkUserIsProviderQuery;
        private readonly UserManager<AppUser> _userManager;
        private readonly IGetAllEmailServiceQuery _getAllEmailServiceQuery;
        private readonly IOptions<EmailConfigModel> _config;
        private readonly IConfiguration _configuration;

        public RegisterProviderServiceCommand(
            IRepository<Provider, Guid> providerRepository,
            IHttpContextAccessor httpContextAccessor,
            ICheckUserIsAdminQuery checkUserIsAdminQuery,
            IGetPermissionActionQuery getPermissionActionQuery,
            ICheckUserIsProviderQuery checkUserIsProviderQuery,
            UserManager<AppUser> userManager,
            IGetAllEmailServiceQuery getAllEmailServiceQuery,
            IOptions<EmailConfigModel> config,
            IConfiguration configuration)
        {
            _providerRepository = providerRepository;
            _httpContextAccessor = httpContextAccessor;
            _checkUserIsAdminQuery = checkUserIsAdminQuery;
            _getPermissionActionQuery = getPermissionActionQuery;
            _checkUserIsProviderQuery = checkUserIsProviderQuery;
            _userManager = userManager;
            _getAllEmailServiceQuery = getAllEmailServiceQuery;
            _config = config;
            _configuration = configuration;
        }

        public async Task<CommandResult<ProviderServiceViewModel>> ExecuteAsync(ProviderServiceViewModel vm)
        {
            var userId = _httpContextAccessor.HttpContext.User.Identity.Name;
            var userName = _userManager.FindByIdAsync(userId).Result.UserName;
            try
            {
                //Check category has available
                var availableCategory = _providerRepository.FindSingleAsync(x => x.ProviderName.ToLower() == vm.ProviderName.ToLower());
                if (availableCategory == null)
                {
                    return new CommandResult<ProviderServiceViewModel>
                    {
                        isValid = false,
                        errorMessage = "Provider Name has available"
                    };
                }
                var checkUserIsProvider = await _checkUserIsProviderQuery.ExecuteAsync(userId);
                var mappingProvider = await MappingProvider(vm, Guid.Parse(userId), userId);

                var userEmail = "";
                if (vm.UserId == null)
                {
                    var findUser = await _userManager.FindByIdAsync(userId);
                    userEmail = findUser.Email;
                }
                else
                {
                    var findUser = await _userManager.FindByIdAsync(vm.UserId);
                    userEmail = findUser.Email;
                }
                mappingProvider.OTPConfirm = RandomCodeSupport.RandomString(6);

                await _providerRepository.Add(mappingProvider);

                await _providerRepository.SaveAsync();
                if ((await _getPermissionActionQuery.ExecuteAsync(userId, ConstantFunctions.PROVIDER, ActionSetting.CanCreate)
                    || await _checkUserIsAdminQuery.ExecuteAsync(userId)))
                {
                    var findUserId = await _userManager.FindByIdAsync(vm.UserId);
                    var getAllRole =  await _userManager.GetRolesAsync(findUserId);
                    var flagProvider = 0;
                    foreach (var item in getAllRole)
                    {
                        if(item == ConstantRoles.Provider)
                        {
                            flagProvider++;
                        }
                    }
                    if (flagProvider == 0)
                    {
                        await _userManager.AddToRoleAsync(findUserId, ConstantRoles.Provider);
                    }

                    //Set content for email
                    var getEmailContent = await _getAllEmailServiceQuery.ExecuteAsync();
                    var generateCode = _configuration.GetSection("Host").GetSection("LinkConfirmProvider").Value +
                         mappingProvider.OTPConfirm + '_' + mappingProvider.Id;

                    var getFirstEmail = getEmailContent.Where(x => x.Name == EmailName.Approve_Provider).FirstOrDefault();
                    getFirstEmail.Message = getFirstEmail.Message.Replace(EmailKey.UserNameKey, userEmail).Replace(EmailKey.ConfirmLink, generateCode);

                    ContentEmail(_config.Value.SendGridKey, getFirstEmail.Subject,
                                    getFirstEmail.Message, findUserId.Email).Wait();
                }
                vm.Id = mappingProvider.Id.ToString();
                vm.Status = mappingProvider.Status;
                await Logging<RegisterProviderServiceCommand>.
                       InformationAsync(ActionCommand.COMMAND_ADD, userName, JsonConvert.SerializeObject(vm));
                return new CommandResult<ProviderServiceViewModel>
                {
                    isValid = true,
                    myModel = vm
                };
            }
            catch (System.Exception ex)
            {
                await Logging<RegisterProviderServiceCommand>.
                       ErrorAsync(ex, ActionCommand.COMMAND_ADD, userName, "Has error");
                return new CommandResult<ProviderServiceViewModel>
                {
                    isValid = false,
                    myModel = vm,
                    errorMessage = ex.InnerException.ToString()
                };
            }
        }

        private async Task<Provider> MappingProvider(ProviderServiceViewModel vm, Guid userId, string currentUserContext)
        {
            Provider pro = new Provider();
            pro.PhoneNumber = vm.PhoneNumber;
            pro.Status = (await _getPermissionActionQuery.ExecuteAsync(currentUserContext, ConstantFunctions.PROVIDER, ActionSetting.CanCreate)
                || await _checkUserIsAdminQuery.ExecuteAsync(currentUserContext)) ? Status.WaitingApprove : Status.Pending;
            pro.CityId = vm.CityId;
            pro.UserId = vm.UserId == null ? userId : Guid.Parse(vm.UserId);
            pro.TaxCode = vm.TaxCode;
            pro.Description = vm.Description;
            pro.DateCreated = DateTime.Now;
            pro.ProviderName = vm.ProviderName;
            pro.Address = vm.Address;
            pro.AvartarPath = vm.AvatarPath;
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