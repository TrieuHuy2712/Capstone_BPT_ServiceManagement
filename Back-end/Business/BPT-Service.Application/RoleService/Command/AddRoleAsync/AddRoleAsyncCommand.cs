using System.Threading.Tasks;
using BPT_Service.Application.RoleService.ViewModel;
using BPT_Service.Application.PermissionService.Query.CheckUserIsAdmin;
using BPT_Service.Application.PermissionService.Query.GetPermissionAction;
using BPT_Service.Model.Entities;
using BPT_Service.Model.Infrastructure.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Http;
using BPT_Service.Common.Helpers;

namespace BPT_Service.Application.RoleService.Command.AddRoleAsync
{
    public class AddRoleAsyncCommand : IAddRoleAsyncCommand
    {
        private readonly RoleManager<AppRole> _roleManager;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICheckUserIsAdminQuery _checkUserIsAdminQuery;
        private readonly IGetPermissionActionQuery _getPermissionActionQuery;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public AddRoleAsyncCommand(
        RoleManager<AppRole> roleManager,
        IUnitOfWork unitOfWork,
        ICheckUserIsAdminQuery checkUserIsAdminQuery,
        IGetPermissionActionQuery getPermissionActionQuery,
        IHttpContextAccessor httpContextAccessor)
        {
            _roleManager = roleManager;
            _unitOfWork = unitOfWork;
            _getPermissionActionQuery = getPermissionActionQuery;
            _checkUserIsAdminQuery = checkUserIsAdminQuery;
            _httpContextAccessor = httpContextAccessor;
        }
        public async Task<CommandResult<AppRoleViewModel>> ExecuteAync(AppRoleViewModel roleVm)
        {
            try
            {
                var userId = _httpContextAccessor.HttpContext.User.Identity.Name;
                if( await _checkUserIsAdminQuery.ExecuteAsync(userId) || await _getPermissionActionQuery.ExecuteAsync(userId, "APPROLES", ActionSetting.CanCreate))
                {
                    var role = new AppRole()
                    {
                        Name = roleVm.Name,
                        Description = roleVm.Description
                    };
                    var result = await _roleManager.CreateAsync(role);
                    _unitOfWork.Commit();
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
                }else
                {
                    return new CommandResult<AppRoleViewModel>{
                        isValid = false,
                        errorMessage = "You don't have permission"
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