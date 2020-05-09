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

namespace BPT_Service.Application.EmailService.Command.DeleteEmailService
{
    public class DeleteEmailServiceCommand : IDeleteEmailServiceCommand
    {
        private readonly IRepository<Email, int> _emailRepository;
        private readonly ICheckUserIsAdminQuery _checkUserIsAdminQuery;
        private readonly IGetPermissionActionQuery _getPermissionActionQuery;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly UserManager<AppUser> _userManager;

        public DeleteEmailServiceCommand(IRepository<Email, int> emailRepository,
            ICheckUserIsAdminQuery checkUserIsAdminQuery,
            IGetPermissionActionQuery getPermissionActionQuery,
            IHttpContextAccessor httpContextAccessor,
            UserManager<AppUser> userManager
            )
        {
            _emailRepository = emailRepository;
            _checkUserIsAdminQuery = checkUserIsAdminQuery;
            _getPermissionActionQuery = getPermissionActionQuery;
            _httpContextAccessor = httpContextAccessor;
            _userManager = userManager;
        }

        public async Task<CommandResult<Email>> ExecuteAsync(int id)
        {
            var userId = _httpContextAccessor.HttpContext.User.Identity.Name;
            var userName = await _userManager.FindByIdAsync(userId);
            try
            {
                if (await _checkUserIsAdminQuery.ExecuteAsync(userId) || await _getPermissionActionQuery.ExecuteAsync(userId, ConstantFunctions.EMAIL, ActionSetting.CanDelete))
                {
                    var idEmail = await _emailRepository.FindByIdAsync(id);
                    if (idEmail == null)
                    {
                        await Logging<DeleteEmailServiceCommand>
                           .WarningAsync(ActionCommand.COMMAND_DELETE, userName.UserName, ErrorMessageConstant.ERROR_CANNOT_FIND_ID);
                        return new CommandResult<Email>
                        {
                            isValid = false,
                            errorMessage = ErrorMessageConstant.ERROR_CANNOT_FIND_ID
                        };
                    }
                    _emailRepository.Remove(id);
                    await _emailRepository.SaveAsync();
                    await Logging<DeleteEmailServiceCommand>.
                            InformationAsync(ActionCommand.COMMAND_DELETE, userName.UserName, JsonConvert.SerializeObject(idEmail));
                    return new CommandResult<Email>
                    {
                        isValid = true,
                        myModel = idEmail
                    };
                }
                else
                {
                    await Logging<DeleteEmailServiceCommand>
                            .WarningAsync(ActionCommand.COMMAND_DELETE, userName.UserName, ErrorMessageConstant.ERROR_DELETE_PERMISSION);
                    return new CommandResult<Email>
                    {
                        isValid = false,
                        errorMessage = ErrorMessageConstant.ERROR_DELETE_PERMISSION
                    };
                }
            }
            catch (Exception ex)
            {
                await Logging<DeleteEmailServiceCommand>.ErrorAsync(ex, ActionCommand.COMMAND_DELETE, "Has error");
                return new CommandResult<Email>
                {
                    isValid = false,
                    errorMessage = ex.InnerException.Message.ToString()
                };
            }
        }
    }
}