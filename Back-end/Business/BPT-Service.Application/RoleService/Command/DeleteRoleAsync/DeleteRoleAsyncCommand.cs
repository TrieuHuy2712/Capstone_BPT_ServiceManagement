using BPT_Service.Application.PermissionService.Query.CheckUserIsAdmin;
using BPT_Service.Application.PermissionService.Query.GetPermissionAction;
using BPT_Service.Application.RoleService.ViewModel;
using BPT_Service.Common;
using BPT_Service.Common.Helpers;
using BPT_Service.Model.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using System;
using System.Threading.Tasks;

namespace BPT_Service.Application.RoleService.Command.DeleteRoleAsync
{
    public class DeleteRoleAsyncCommand : IDeleteRoleAsyncCommand
    {
        private readonly RoleManager<AppRole> _roleManager;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ICheckUserIsAdminQuery _checkUserIsAdminQuery;
        private readonly IGetPermissionActionQuery _getPermissionActionQuery;

        public DeleteRoleAsyncCommand(
            RoleManager<AppRole> roleManager,
            IHttpContextAccessor httpContextAccessor,
            ICheckUserIsAdminQuery checkUserIsAdminQuery,
            IGetPermissionActionQuery getPermissionActionQuery)
        {
            _roleManager = roleManager;
            _httpContextAccessor = httpContextAccessor;
            _checkUserIsAdminQuery = checkUserIsAdminQuery;
            _getPermissionActionQuery = getPermissionActionQuery;
        }

        public async Task<CommandResult<AppRoleViewModel>> ExecuteAsync(Guid id)
        {
            try
            {
                var userId = _httpContextAccessor.HttpContext.User.Identity.Name;
                if (await _checkUserIsAdminQuery.ExecuteAsync(userId) ||
                    await _getPermissionActionQuery.ExecuteAsync(userId, "ROLE", ActionSetting.CanDelete))
                {
                    var role = await _roleManager.FindByIdAsync(id.ToString());
                    if (role != null)
                    {
                        await _roleManager.DeleteAsync(role);
                        return new CommandResult<AppRoleViewModel>
                        {
                            isValid = true,
                        };
                    }
                    return new CommandResult<AppRoleViewModel>
                    {
                        isValid = false,
                        errorMessage = ErrorMessageConstant.ERROR_CANNOT_FIND_ID
                    };
                }
                else
                {
                    return new CommandResult<AppRoleViewModel>
                    {
                        errorMessage = ErrorMessageConstant.ERROR_DELETE_PERMISSION,
                        isValid = false
                    };
                }
            }
            catch (System.Exception ex)
            {
                return new CommandResult<AppRoleViewModel>
                {
                    isValid = false,
                    errorMessage = ex.InnerException.ToString()
                };
            }
        }
    }
}