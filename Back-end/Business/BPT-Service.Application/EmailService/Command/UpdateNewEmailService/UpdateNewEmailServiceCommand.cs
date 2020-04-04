using BPT_Service.Application.EmailService.ViewModel;
using BPT_Service.Application.PermissionService.Query.CheckUserIsAdmin;
using BPT_Service.Application.PermissionService.Query.GetPermissionAction;
using BPT_Service.Common;
using BPT_Service.Common.Helpers;
using BPT_Service.Common.Logging;
using BPT_Service.Model.Entities;
using BPT_Service.Model.Infrastructure.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Newtonsoft.Json;
using System;
using System.Threading.Tasks;

namespace BPT_Service.Application.EmailService.Command.UpdateNewEmailService
{
    public class UpdateNewEmailServiceCommand : IUpdateNewEmailServiceCommand
    {
        private readonly IRepository<Email, int> _emailRepository;
        private readonly ICheckUserIsAdminQuery _checkUserIsAdminQuery;
        private readonly IGetPermissionActionQuery _getPermissionActionQuery;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly UserManager<AppUser> _userManager;

        public UpdateNewEmailServiceCommand(IRepository<Email, int> emailRepository,
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

        public async Task<CommandResult<Email>> ExecuteAsync(EmailViewModel emailViewModel)
        {
            var userId = _httpContextAccessor.HttpContext.User.Identity.Name;
            var userName = _userManager.FindByIdAsync(userId).Result.UserName;
            try
            {
                if (await _checkUserIsAdminQuery.ExecuteAsync(userId) || await _getPermissionActionQuery.ExecuteAsync(userId, "EMAIL", ActionSetting.CanUpdate))
                {
                    var checkId = await _emailRepository.FindByIdAsync(emailViewModel.Id);
                    if (checkId == null)
                    {
                        await Logging<UpdateNewEmailServiceCommand>.WarningAsync(ActionCommand.COMMAND_UPDATE, userName, ErrorMessageConstant.ERROR_CANNOT_FIND_ID);
                        return new CommandResult<Email>
                        {
                            isValid = false,
                            errorMessage = ErrorMessageConstant.ERROR_CANNOT_FIND_ID
                        };
                    }
                    var mappingEmail = MappingEmail(checkId, emailViewModel);
                    _emailRepository.Update(mappingEmail);
                    await _emailRepository.SaveAsync();
                    await Logging<UpdateNewEmailServiceCommand>.
                        InformationAsync(ActionCommand.COMMAND_UPDATE, userName, JsonConvert.SerializeObject(mappingEmail));
                    return new CommandResult<Email>
                    {
                        isValid = true,
                        myModel = checkId
                    };
                }
                else
                {
                    await Logging<UpdateNewEmailServiceCommand>.
                        WarningAsync(ActionCommand.COMMAND_UPDATE, userName, ErrorMessageConstant.ERROR_UPDATE_PERMISSION);
                    return new CommandResult<Email>
                    {
                        isValid = false,
                        errorMessage = ErrorMessageConstant.ERROR_UPDATE_PERMISSION
                    };
                }
            }
            catch (Exception ex)
            {
                await Logging<UpdateNewEmailServiceCommand>.
                       ErrorAsync(ex, ActionCommand.COMMAND_UPDATE, userName, "Has error");
                return new CommandResult<Email>
                {
                    isValid = false,
                    errorMessage = ex.InnerException.Message.ToString()
                };
            }
        }

        private Email MappingEmail(Email email, EmailViewModel emailViewModel)
        {
            email.Message = emailViewModel.Message;
            email.Name = emailViewModel.Name;
            email.Subject = emailViewModel.Subject;
            email.To = emailViewModel.To;
            return email;
        }
    }
}