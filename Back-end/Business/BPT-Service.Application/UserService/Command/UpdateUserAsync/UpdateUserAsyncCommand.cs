using BPT_Service.Application.PermissionService.Query.CheckUserIsAdmin;
using BPT_Service.Application.PermissionService.Query.GetPermissionAction;
using BPT_Service.Application.UserService.ViewModel;
using BPT_Service.Common;
using BPT_Service.Common.Helpers;
using BPT_Service.Model.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace BPT_Service.Application.UserService.Command.UpdateUserAsync
{
    public class UpdateUserAsyncCommand : IUpdateUserAsyncCommand
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ICheckUserIsAdminQuery _checkUserIsAdminQuery;
        private readonly IGetPermissionActionQuery _getPermissionActionQuery;

        public UpdateUserAsyncCommand(
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

        public async Task<CommandResult<AppUserViewModelinUserService>> ExecuteAsync(AppUserViewModelinUserService userVm)
        {
            try
            {
                var userId = _httpContextAccessor.HttpContext.User.Identity.Name;
                if (await _checkUserIsAdminQuery.ExecuteAsync(userId) ||
                    await _getPermissionActionQuery.ExecuteAsync(userId, "USER", ActionSetting.CanUpdate))
                {
                    var user = await _userManager.FindByIdAsync(userVm.Id.ToString());
                    user.FullName = userVm.FullName;
                    user.Status = userVm.Status;
                    user.Email = userVm.Email;
                    user.PhoneNumber = userVm.PhoneNumber;
                    var userRoles = _userManager.GetRolesAsync(user);

                    var selectedRole = userVm.NewRoles.ToArray();
                    selectedRole = selectedRole ?? new string[] { };

                    await _userManager.AddToRolesAsync(user, selectedRole.Except(userRoles.Result).ToArray());
                    var userRoles1 = await _userManager.GetRolesAsync(user);
                    await _userManager.UpdateAsync(user);
                    return new CommandResult<AppUserViewModelinUserService>
                    {
                        isValid = true,
                        myModel = new AppUserViewModelinUserService
                        {
                            FullName = userVm.FullName,
                            Email = userVm.Email,
                            PhoneNumber = userVm.PhoneNumber,
                            Roles = userVm.Roles.ToList()
                        }
                    };
                }
                else
                {
                    return new CommandResult<AppUserViewModelinUserService>
                    {
                        isValid = false,
                        errorMessage = ErrorMessageConstant.ERROR_UPDATE_PERMISSION
                    };
                }
            }
            catch (Exception ex)
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