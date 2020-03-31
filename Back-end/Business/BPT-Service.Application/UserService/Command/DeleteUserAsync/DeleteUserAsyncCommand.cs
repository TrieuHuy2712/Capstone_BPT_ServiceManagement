using System.Threading.Tasks;
using BPT_Service.Application.PermissionService.Query.CheckUserIsAdmin;
using BPT_Service.Application.PermissionService.Query.GetPermissionAction;
using BPT_Service.Application.UserService.ViewModel;
using BPT_Service.Common;
using BPT_Service.Common.Helpers;
using BPT_Service.Model.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;

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
            try
            {
                var userId = _httpContextAccessor.HttpContext.User.Identity.Name;
                if (await _checkUserIsAdminQuery.ExecuteAsync(userId) ||
                    await _getPermissionActionQuery.ExecuteAsync(userId, "USER", ActionSetting.CanDelete))
                {
                    var user = await _userManager.FindByIdAsync(id);
                    if (user != null)
                    {
                        await _userManager.DeleteAsync(user);
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
                    return new CommandResult<AppUserViewModelinUserService>
                    {
                        isValid = false,
                        errorMessage = ErrorMessageConstant.ERROR_ADD_PERMISSION
                    };
                }
                else
                {
                    return new CommandResult<AppUserViewModelinUserService>
                    {
                        isValid = false,
                        errorMessage = ErrorMessageConstant.ERROR_DELETE_PERMISSION
                    };
                }
            }
            catch (System.Exception ex)
            {
                return new CommandResult<AppUserViewModelinUserService>
                {
                    isValid = false,
                    errorMessage = ex.InnerException.ToString()
                };
            }
        }
    }
}