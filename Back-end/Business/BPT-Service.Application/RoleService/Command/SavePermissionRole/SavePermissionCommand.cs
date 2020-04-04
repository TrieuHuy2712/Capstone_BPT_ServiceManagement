using BPT_Service.Application.PermissionService.Query.CheckUserIsAdmin;
using BPT_Service.Application.PermissionService.Query.GetPermissionAction;
using BPT_Service.Application.RoleService.ViewModel;
using BPT_Service.Common;
using BPT_Service.Common.Helpers;
using BPT_Service.Common.Logging;
using BPT_Service.Model.Entities;
using BPT_Service.Model.Infrastructure.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Newtonsoft.Json;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace BPT_Service.Application.RoleService.Command.SavePermissionRole
{
    public class SavePermissionCommand : ISavePermissionCommand
    {
        private readonly IRepository<Permission, int> _permissionRepository;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ICheckUserIsAdminQuery _checkUserIsAdminQuery;
        private readonly IGetPermissionActionQuery _getPermissionActionQuery;
        private readonly UserManager<AppUser> _userManager;

        public SavePermissionCommand(
            IRepository<Permission, int> permissionRepository,
            IHttpContextAccessor httpContextAccessor,
            ICheckUserIsAdminQuery checkUserIsAdminQuery,
            IGetPermissionActionQuery getPermissionActionQuery,
            UserManager<AppUser> userManager)
        {
            _permissionRepository = permissionRepository;
            _httpContextAccessor = httpContextAccessor;
            _checkUserIsAdminQuery = checkUserIsAdminQuery;
            _getPermissionActionQuery = getPermissionActionQuery;
            _userManager = userManager;
        }

        public async Task<CommandResult<RolePermissionViewModel>> ExecuteAsync(RolePermissionViewModel rolePermissionViewModel)
        {
            var userId = _httpContextAccessor.HttpContext.User.Identity.Name;
            var userName = _userManager.FindByIdAsync(userId).Result.UserName;
            try
            {
                if (await _checkUserIsAdminQuery.ExecuteAsync(userId) ||
                    await _getPermissionActionQuery.ExecuteAsync(userId, "ROLE", ActionSetting.CanCreate))
                {
                    var permissions = rolePermissionViewModel.Permissions.Select(x => new Permission
                    {
                        CanCreate = x.CanCreate,
                        CanDelete = x.CanDelete,
                        CanRead = x.CanRead,
                        CanUpdate = x.CanUpdate,
                        FunctionId = rolePermissionViewModel.FunctionId,
                        RoleId = x.RoleId
                    }).ToList();
                    var oldPermission = await _permissionRepository.FindAllAsync(x => x.FunctionId == rolePermissionViewModel.FunctionId);
                    if (oldPermission.Count() > 0)
                    {
                        _permissionRepository.RemoveMultiple(oldPermission.ToList());
                    }
                    foreach (var permission in permissions)
                    {
                        await _permissionRepository.Add(permission);
                    }
                    await _permissionRepository.SaveAsync();
                    await Logging<SavePermissionCommand>.InformationAsync(ActionCommand.COMMAND_ADD,userName,JsonConvert.SerializeObject(permissions));
                    return new CommandResult<RolePermissionViewModel>
                    {
                        isValid = true,
                        myModel = new RolePermissionViewModel
                        {
                            FunctionId = rolePermissionViewModel.FunctionId,
                            Permissions = permissions.Select(x => new PermissionViewModel
                            {
                                CanCreate = x.CanCreate,
                                CanDelete = x.CanDelete,
                                CanRead = x.CanRead,
                                CanUpdate = x.CanUpdate,
                                FunctionId = rolePermissionViewModel.FunctionId,
                                RoleId = x.RoleId
                            }).ToList()
                        }
                    };
                }
                else
                {
                    await Logging<SavePermissionCommand>.
                        WarningAsync(ActionCommand.COMMAND_ADD, userName, ErrorMessageConstant.ERROR_ADD_PERMISSION);
                    return new CommandResult<RolePermissionViewModel>
                    {
                        isValid = false,
                        errorMessage = ErrorMessageConstant.ERROR_ADD_PERMISSION
                    };
                }
            }
            catch (System.Exception ex)
            {
                await Logging<SavePermissionCommand>.ErrorAsync(ex, ActionCommand.COMMAND_ADD, userName, "Has error");
                return new CommandResult<RolePermissionViewModel>
                {
                    isValid = false,
                    errorMessage = ex.InnerException.ToString()
                };
            }
        }
    }
}