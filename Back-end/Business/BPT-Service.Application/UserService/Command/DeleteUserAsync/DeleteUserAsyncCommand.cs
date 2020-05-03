using BPT_Service.Application.PermissionService.Query.CheckUserIsAdmin;
using BPT_Service.Application.PermissionService.Query.GetPermissionAction;
using BPT_Service.Application.UserService.ViewModel;
using BPT_Service.Common;
using BPT_Service.Common.Constants;
using BPT_Service.Common.Helpers;
using BPT_Service.Common.Logging;
using BPT_Service.Model.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Newtonsoft.Json;
using System.Security.Claims;
using System.Threading.Tasks;

namespace BPT_Service.Application.UserService.Command.DeleteUserAsync
{
    public class DeleteUserAsyncCommand : IDeleteUserAsyncCommand
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ICheckUserIsAdminQuery _checkUserIsAdminQuery;
        private readonly IGetPermissionActionQuery _getPermissionActionQuery;

        public DeleteUserAsyncCommand(
            UserManager<AppUser> userManager,
            IHttpContextAccessor httpContextAccessor,
            ICheckUserIsAdminQuery checkUserIsAdminQuery,
            IGetPermissionActionQuery getPermissionActionQuery)
        {
            _userManager = userManager;
            _httpContextAccessor = httpContextAccessor;
            _checkUserIsAdminQuery = checkUserIsAdminQuery;
            _getPermissionActionQuery = getPermissionActionQuery;
        }

        public async Task<CommandResult<AppUserViewModelinUserService>> ExecuteAsync(string id)
        {
            var userId = _httpContextAccessor.HttpContext.User.Identity.Name;
            var userName = _userManager.FindByIdAsync(userId).Result.UserName;
            try
            {
                if (await _checkUserIsAdminQuery.ExecuteAsync(userId) ||
                    await _getPermissionActionQuery.ExecuteAsync(userId, ConstantFunctions.USER, ActionSetting.CanDelete))
                {
                    var user = await _userManager.FindByIdAsync(id);
                    if (user != null)
                    {
                        await _userManager.DeleteAsync(user);
                        await Logging<DeleteUserAsyncCommand>.InformationAsync(ActionCommand.COMMAND_DELETE, userName, JsonConvert.SerializeObject(user));
                        return new CommandResult<AppUserViewModelinUserService>
                        {
                            isValid = true,
                            myModel = new AppUserViewModelinUserService
                            {
                                UserName = user.UserName,
                                Avatar = user.Avatar,
                                Email = user.Email,
                                FullName = user.FullName,
                                DateCreated = user.DateCreated,
                            }
                        };
                    }
                    await Logging<DeleteUserAsyncCommand>.
                        WarningAsync(ActionCommand.COMMAND_DELETE, userName, ErrorMessageConstant.ERROR_ADD_PERMISSION);
                    return new CommandResult<AppUserViewModelinUserService>
                    {
                        isValid = false,
                        errorMessage = ErrorMessageConstant.ERROR_ADD_PERMISSION
                    };
                }
                else
                {
                    await Logging<DeleteUserAsyncCommand>.
                        WarningAsync(ActionCommand.COMMAND_DELETE, userName, ErrorMessageConstant.ERROR_DELETE_PERMISSION);
                    return new CommandResult<AppUserViewModelinUserService>
                    {
                        isValid = false,
                        errorMessage = ErrorMessageConstant.ERROR_DELETE_PERMISSION
                    };
                }
            }
            catch (System.Exception ex)
            {
                await Logging<DeleteUserAsyncCommand>.
                        ErrorAsync(ex, ActionCommand.COMMAND_DELETE, userName, "Has error");
                return new CommandResult<AppUserViewModelinUserService>
                {
                    isValid = false,
                    errorMessage = ex.InnerException.ToString()
                };
            }
        }
    }
}