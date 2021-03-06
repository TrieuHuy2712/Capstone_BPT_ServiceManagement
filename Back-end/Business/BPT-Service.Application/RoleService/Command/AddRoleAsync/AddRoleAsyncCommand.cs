using BPT_Service.Application.PermissionService.Query.CheckUserIsAdmin;
using BPT_Service.Application.PermissionService.Query.GetPermissionAction;
using BPT_Service.Application.RoleService.ViewModel;
using BPT_Service.Common;
using BPT_Service.Common.Constants;
using BPT_Service.Common.Helpers;
using BPT_Service.Common.Logging;
using BPT_Service.Model.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Newtonsoft.Json;
using System.Threading.Tasks;

namespace BPT_Service.Application.RoleService.Command.AddRoleAsync
{
    public class AddRoleAsyncCommand : IAddRoleAsyncCommand
    {
        private readonly RoleManager<AppRole> _roleManager;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ICheckUserIsAdminQuery _checkUserIsAdminQuery;
        private readonly IGetPermissionActionQuery _getPermissionActionQuery;
        private readonly UserManager<AppUser> _userManager;

        public AddRoleAsyncCommand(
            RoleManager<AppRole> roleManager,
            IHttpContextAccessor httpContextAccessor,
            ICheckUserIsAdminQuery checkUserIsAdminQuery,
            IGetPermissionActionQuery getPermissionActionQuery,
            UserManager<AppUser> userManager)
        {
            _roleManager = roleManager;
            _httpContextAccessor = httpContextAccessor;
            _checkUserIsAdminQuery = checkUserIsAdminQuery;
            _getPermissionActionQuery = getPermissionActionQuery;
            _userManager = userManager;
        }

        public async Task<CommandResult<AppRoleViewModel>> ExecuteAync(AppRoleViewModel roleVm)
        {
            var userId = _httpContextAccessor.HttpContext.User.Identity.Name;
            var userName = _userManager.FindByIdAsync(userId).Result.UserName;
            try
            {
                if (await _checkUserIsAdminQuery.ExecuteAsync(userId) ||
                    await _getPermissionActionQuery.ExecuteAsync(userId, ConstantFunctions.ROLE, ActionSetting.CanCreate))
                {
                    var role = new AppRole()
                    {
                        Name = roleVm.Name,
                        Description = roleVm.Description
                    };
                    var result = await _roleManager.CreateAsync(role);
                    await Logging<AddRoleAsyncCommand>.InformationAsync(ActionCommand.COMMAND_ADD, userName, JsonConvert.SerializeObject(role));
                    return new CommandResult<AppRoleViewModel>
                    {
                        isValid = result.Succeeded,
                        myModel = new AppRoleViewModel
                        {
                            Description = role.Description,
                            Name = role.Name,
                            Id = role.Id
                        }
                    };
                }
                else
                {
                    await Logging<AddRoleAsyncCommand>.WarningAsync(ActionCommand.COMMAND_ADD, userName, ErrorMessageConstant.ERROR_ADD_PERMISSION);
                    return new CommandResult<AppRoleViewModel>
                    {
                        isValid = false,
                        errorMessage = ErrorMessageConstant.ERROR_ADD_PERMISSION
                    };
                }
            }
            catch (System.Exception ex)
            {
                await Logging<AddRoleAsyncCommand>.ErrorAsync(ex, ActionCommand.COMMAND_ADD, userName, "Has error");
                return new CommandResult<AppRoleViewModel>
                {
                    isValid = false,
                    errorMessage = ex.InnerException.ToString()
                };
            }
        }
    }
}