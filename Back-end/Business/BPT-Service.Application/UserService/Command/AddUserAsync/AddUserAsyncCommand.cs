using BPT_Service.Application.PermissionService.Query.CheckUserIsAdmin;
using BPT_Service.Application.PermissionService.Query.GetPermissionAction;
using BPT_Service.Application.UserService.ViewModel;
using BPT_Service.Common;
using BPT_Service.Common.Helpers;
using BPT_Service.Model.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using System;
using System.Threading.Tasks;

namespace BPT_Service.Application.UserService.Command.AddUserAsync
{
    public class AddUserAsyncCommand : IAddUserAsyncCommand
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ICheckUserIsAdminQuery _checkUserIsAdminQuery;
        private readonly IGetPermissionActionQuery _getPermissionActionQuery;

        public AddUserAsyncCommand(
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
                    await _getPermissionActionQuery.ExecuteAsync(userId, "USER", ActionSetting.CanCreate))
                {
                    var user = new AppUser()
                    {
                        UserName = userVm.UserName,
                        Avatar = userVm.Avatar,
                        Email = userVm.Email,
                        FullName = userVm.FullName,
                        DateCreated = DateTime.Now,
                        PhoneNumber = userVm.PhoneNumber
                    };
                    var result = await _userManager.CreateAsync(user, userVm.Password);
                    if (result.Succeeded && userVm.Roles.Count > 0)
                    {
                        var appUser = await _userManager.FindByNameAsync(user.UserName);
                        if (appUser != null)
                            await _userManager.AddToRolesAsync(appUser, userVm.Roles);
                    }
                    return new CommandResult<AppUserViewModelinUserService>
                    {
                        isValid = true,
                        myModel = new AppUserViewModelinUserService
                        {
                            UserName = userVm.UserName,
                            Avatar = userVm.Avatar,
                            Email = userVm.Email,
                            FullName = userVm.FullName,
                            DateCreated = DateTime.Now,
                            PhoneNumber = userVm.PhoneNumber
                        }
                    };
                }
                else
                {
                    return new CommandResult<AppUserViewModelinUserService>
                    {
                        isValid = false,
                        errorMessage = ErrorMessageConstant.ERROR_ADD_PERMISSION
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