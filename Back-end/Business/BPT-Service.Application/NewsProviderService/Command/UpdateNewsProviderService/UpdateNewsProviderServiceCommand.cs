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
using BPT_Service.Model.Entities;
using BPT_Service.Model.Entities.ServiceModel;
using BPT_Service.Model.Entities.ServiceModel.ProviderServiceModel;
using BPT_Service.Model.Enums;
using BPT_Service.Model.Infrastructure.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using SendGrid;
using SendGrid.Helpers.Mail;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace BPT_Service.Application.NewsProviderService.Command.UpdateNewsProviderService
{
    public class UpdateNewsProviderServiceCommand : IUpdateNewsProviderServiceCommand
    {
        private readonly IRepository<ProviderNew, int> _providerNewsRepository;
        private readonly IHttpContextAccessor _httpContext;
        private readonly ICheckUserIsAdminQuery _checkUserIsAdminQuery;
        private readonly IGetPermissionActionQuery _getPermissionActionQuery;
        private readonly ICheckUserIsProviderQuery _checkUserIsProviderQuery;
        private readonly UserManager<AppUser> _userManager;
        private readonly IGetAllEmailServiceQuery _getAllEmailServiceQuery;
        private readonly IConfiguration _configuration;
        private readonly IOptions<EmailConfigModel> _config;
        private readonly IRepository<Provider, Guid> _providerRepository;

        public UpdateNewsProviderServiceCommand(IRepository<ProviderNew, int> providerNewsRepository, IHttpContextAccessor httpContext, ICheckUserIsAdminQuery checkUserIsAdminQuery, IGetPermissionActionQuery getPermissionActionQuery, ICheckUserIsProviderQuery checkUserIsProviderQuery, UserManager<AppUser> userManager, IGetAllEmailServiceQuery getAllEmailServiceQuery, IConfiguration configuration, IOptions<EmailConfigModel> config, IRepository<Provider, Guid> providerRepository)
        {
            _providerNewsRepository = providerNewsRepository;
            _httpContext = httpContext;
            _checkUserIsAdminQuery = checkUserIsAdminQuery;
            _getPermissionActionQuery = getPermissionActionQuery;
            _checkUserIsProviderQuery = checkUserIsProviderQuery;
            _userManager = userManager;
            _getAllEmailServiceQuery = getAllEmailServiceQuery;
            _configuration = configuration;
            _config = config;
            _providerRepository = providerRepository;
        }

        public async Task<CommandResult<NewsProviderViewModel>> ExecuteAsync(NewsProviderViewModel vm)
        {
            var userId = _httpContext.HttpContext.User.Identity.Name;
            var userName = _userManager.FindByIdAsync(userId).Result.UserName;
            try
            {
                var findProviderNew = await _providerNewsRepository.FindByIdAsync(vm.Id);
                if (findProviderNew != null)
                {
                    var getIsProvider = await _checkUserIsProviderQuery.ExecuteAsync(userId);
                    if (await _checkUserIsAdminQuery.ExecuteAsync(userId) ||
                       await _getPermissionActionQuery.ExecuteAsync(userId, ConstantFunctions.NEWS, ActionSetting.CanUpdate) ||
                       (getIsProvider.isValid && getIsProvider.myModel.Id == findProviderNew.ProviderId.ToString()))
                    {
                        var mappingNewsProvider = await MappingProvider(findProviderNew, vm, userId);
                        _providerNewsRepository.Update(mappingNewsProvider);
                        await _providerNewsRepository.SaveAsync();
                        await Logging<UpdateNewsProviderServiceCommand>.
                            InformationAsync(ActionCommand.COMMAND_UPDATE, userName, mappingNewsProvider.Author + "had been updated");
                        //Get Provider Information
                        var providerInformation = await _providerRepository.FindByIdAsync(Guid.Parse(vm.ProviderId));
                        var findUser = await _userManager.FindByIdAsync(providerInformation.UserId.ToString());
                        //Set content for email
                        var getEmailContent = await _getAllEmailServiceQuery.ExecuteAsync();
                        var generateCode = _configuration.GetSection("Host").GetSection("LinkConfirmNewsProvider").Value +
                             mappingNewsProvider.CodeConfirm + '_' + mappingNewsProvider.Id;

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
                        await Logging<UpdateNewsProviderServiceCommand>.
                            WarningAsync(ActionCommand.COMMAND_UPDATE, userName, ErrorMessageConstant.ERROR_UPDATE_PERMISSION);
                        return new CommandResult<NewsProviderViewModel>
                        {
                            isValid = false,
                            errorMessage = ErrorMessageConstant.ERROR_UPDATE_PERMISSION
                        };
                    }
                }
                else
                {
                    await Logging<UpdateNewsProviderServiceCommand>.
                            WarningAsync(ActionCommand.COMMAND_UPDATE, userName, ErrorMessageConstant.ERROR_CANNOT_FIND_ID);
                    return new CommandResult<NewsProviderViewModel>
                    {
                        isValid = false,
                        errorMessage = ErrorMessageConstant.ERROR_CANNOT_FIND_ID
                    };
                }
            }
            catch (System.Exception ex)
            {
                await Logging<UpdateNewsProviderServiceCommand>.
                        ErrorAsync(ex, ActionCommand.COMMAND_UPDATE, userName, "Has error");
                return new CommandResult<NewsProviderViewModel>
                {
                    isValid = false,
                    errorMessage = ex.InnerException.ToString()
                };
            }
        }

        private async Task<ProviderNew> MappingProvider(ProviderNew pro, NewsProviderViewModel vm, string currentUserContext)
        {
            pro.Author = vm.Author;
            pro.Status = (await _checkUserIsAdminQuery.ExecuteAsync(currentUserContext) ||
                            await _getPermissionActionQuery.ExecuteAsync(currentUserContext, ConstantFunctions.NEWS, ActionSetting.CanUpdate)) ?
                            Status.WaitingApprove : Status.Pending;
            pro.Author = vm.Author;
            pro.ProviderId = Guid.Parse(vm.ProviderId);
            pro.Title = vm.Title;
            pro.Content = vm.Content;
            pro.DateModified = DateTime.Now;
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