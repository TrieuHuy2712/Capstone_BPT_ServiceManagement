using BPT_Service.Application.PermissionService.Query.CheckUserIsAdmin;
using BPT_Service.Application.PermissionService.Query.GetPermissionAction;
using BPT_Service.Application.UserService.ViewModel;
using BPT_Service.Common;
using BPT_Service.Common.Helpers;
using BPT_Service.Common.Logging;
using BPT_Service.Model.Entities;
using BPT_Service.Model.IRepositories;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace BPT_Service.Application.UserService.Command.UpdateUserAsync
{
    public class UpdateUserAsyncCommand : IUpdateUserAsyncCommand
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ICheckUserIsAdminQuery _checkUserIsAdminQuery;
        private readonly IGetPermissionActionQuery _getPermissionActionQuery;
        private readonly IUserRoleRepository _userRoleRepository;
        private readonly RoleManager<AppRole> _roleRepository;

        public UpdateUserAsyncCommand(
            UserManager<AppUser> userManager, 
            IHttpContextAccessor httpContextAccessor, 
            ICheckUserIsAdminQuery checkUserIsAdminQuery, 
            IGetPermissionActionQuery getPermissionActionQuery, 
            IUserRoleRepository userRoleRepository, 
            RoleManager<AppRole> roleRepository)
        {
            _userManager = userManager;
            _httpContextAccessor = httpContextAccessor;
            _checkUserIsAdminQuery = checkUserIsAdminQuery;
            _getPermissionActionQuery = getPermissionActionQuery;
            _userRoleRepository = userRoleRepository;
            _roleRepository = roleRepository;
        }

        public async Task<CommandResult<AppUserViewModelinUserService>> ExecuteAsync(AppUserViewModelinUserService userVm)
        {
            var userId = _httpContextAccessor.HttpContext.User.Identity.Name;
            var userName = _userManager.FindByIdAsync(userId).Result.UserName;
            try
            {
                if (await _checkUserIsAdminQuery.ExecuteAsync(userId) ||
                    await _getPermissionActionQuery.ExecuteAsync(userId, "USER", ActionSetting.CanUpdate))
                {
                    var user = await _userManager.FindByIdAsync(userVm.Id.ToString());
                    user.FullName = userVm.FullName;
                    user.Status = userVm.Status;
                    user.Email = userVm.Email;
                    user.PhoneNumber = userVm.PhoneNumber;
                    user.Avatar = userVm.Avatar;
                    user.UserName = userVm.UserName;
                    var userRoles = _userManager.GetRolesAsync(user);
                    List<Guid> roleId = new List<Guid>();
                    foreach (var item in userRoles.Result.ToList())
                    {
                        var getId = _roleRepository.FindByNameAsync(item).Result.Id;
                        roleId.Add(getId);
                    }
                    foreach (var item in roleId)
                    {
                        _userRoleRepository.DeleteUserRole(user.Id, item);
                    }
                    var selectedRole = userVm.NewRoles.ToArray();
                    selectedRole = selectedRole ?? new string[] { };

                    //await _userManager.AddToRolesAsync(user, selectedRole.Except(userRoles.Result).ToArray());
                    var userRoles1 = await _userManager.GetRolesAsync(user);
                    await _userManager.UpdateAsync(user);
                    await Logging<UpdateUserAsyncCommand>.InformationAsync(ActionCommand.COMMAND_UPDATE, userName, JsonConvert.SerializeObject(user));
                    return new CommandResult<AppUserViewModelinUserService>
                    {
                        isValid = true,
                        myModel = new AppUserViewModelinUserService
                        {
                            FullName = userVm.FullName,
                            Email = userVm.Email,
                            PhoneNumber = userVm.PhoneNumber,
                            Id = userVm.Id,
                            Avatar= userVm.Avatar,
                            DateCreated= user.DateCreated,
                            UserName= user.UserName,
                            Roles = userVm.Roles.ToList()
                        }
                    };
                }
                else
                {
                    await Logging<UpdateUserAsyncCommand>.
                        WarningAsync(ActionCommand.COMMAND_UPDATE, userName, ErrorMessageConstant.ERROR_UPDATE_PERMISSION);
                    return new CommandResult<AppUserViewModelinUserService>
                    {
                        isValid = false,
                        errorMessage = ErrorMessageConstant.ERROR_UPDATE_PERMISSION
                    };
                }
            }
            catch (Exception ex)
            {
                await Logging<UpdateUserAsyncCommand>.
                        ErrorAsync(ex, ActionCommand.COMMAND_UPDATE, userName, "Has error");
                return new CommandResult<AppUserViewModelinUserService>
                {
                    isValid = false,
                    errorMessage = ex.InnerException.ToString()
                };
            }
        }
    }
}