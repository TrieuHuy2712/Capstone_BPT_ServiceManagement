using BPT_Service.Application.CategoryService.Command.AddCategoryService;
using BPT_Service.Application.EmailService.ViewModel;
using BPT_Service.Application.PermissionService.Query.CheckUserIsAdmin;
using BPT_Service.Application.PermissionService.Query.GetPermissionAction;
using BPT_Service.Common;
using BPT_Service.Common.Constants;
using BPT_Service.Common.Helpers;
using BPT_Service.Common.Logging;
using BPT_Service.Model.Entities;
using BPT_Service.Model.Infrastructure.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Newtonsoft.Json;
using System;
using System.Security.Claims;
using System.Threading.Tasks;

namespace BPT_Service.Application.EmailService.Command.AddNewEmailService
{
    public class AddNewEmailServiceCommand : IAddNewEmailServiceCommand
    {
        private readonly IRepository<Email, int> _emailRepository;
        private readonly ICheckUserIsAdminQuery _checkUserIsAdminQuery;
        private readonly IGetPermissionActionQuery _getPermissionActionQuery;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly UserManager<AppUser> _userManager;

        public AddNewEmailServiceCommand(IRepository<Email, int> emailRepository,
            ICheckUserIsAdminQuery checkUserIsAdminQuery,
            IGetPermissionActionQuery getPermissionActionQuery,
            IHttpContextAccessor httpContextAccessor,
            UserManager<AppUser> userManager)
        {
            _emailRepository = emailRepository;
            _checkUserIsAdminQuery = checkUserIsAdminQuery;
            _getPermissionActionQuery = getPermissionActionQuery;
            _httpContextAccessor = httpContextAccessor;
            _userManager = userManager;
        }

        public async Task<CommandResult<Email>> ExecuteAsync(EmailViewModel emailVIewModel)
        {

            var userId = _httpContextAccessor.HttpContext.User.Identity.Name;
            var userName = await _userManager.FindByIdAsync(userId);
            try
            {
                //Check user has permission first
                if (await _checkUserIsAdminQuery.ExecuteAsync(userId) || await _getPermissionActionQuery.ExecuteAsync(userId, ConstantFunctions.EMAIL, ActionSetting.CanCreate))
                {
                    var mappingEmail = MappingEmail(emailVIewModel);
                    await _emailRepository.Add(mappingEmail);
                    await _emailRepository.SaveAsync();
                    await Logging<AddNewEmailServiceCommand>.
                       InformationAsync(ActionCommand.COMMAND_ADD, userName.UserName, JsonConvert.SerializeObject(mappingEmail));
                    return new CommandResult<Email>
                    {
                        isValid = true,
                        myModel = mappingEmail
                    };
                }
                else
                {
                    await Logging<AddNewEmailServiceCommand>.
                        WarningAsync(ActionCommand.COMMAND_ADD, userName.UserName, ErrorMessageConstant.ERROR_ADD_PERMISSION);
                    return new CommandResult<Email>
                    {
                        isValid = false,
                        errorMessage = ErrorMessageConstant.ERROR_ADD_PERMISSION
                    };
                }
            }
            catch (Exception ex)
            {
                await Logging<AddNewEmailServiceCommand>.ErrorAsync(ex, ActionCommand.COMMAND_ADD, userName.UserName, "You have error");
                return new CommandResult<Email>
                {
                    isValid = false,
                    errorMessage = ex.InnerException.Message.ToString()
                };
            }
        }

        private Email MappingEmail(EmailViewModel emailViewModel)
        {
            Email email = new Email();
            email.Message = emailViewModel.Message;
            email.Name = emailViewModel.Name;
            email.Subject = emailViewModel.Subject;
            email.To = emailViewModel.To;
            return email;
        }
    }
}