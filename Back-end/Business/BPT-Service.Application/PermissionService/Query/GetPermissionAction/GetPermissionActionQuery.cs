using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BPT_Service.Application.RoleService.ViewModel;
using BPT_Service.Common.Helpers;
using BPT_Service.Model.Entities;
using BPT_Service.Model.Infrastructure.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace BPT_Service.Application.PermissionService.Query.GetPermissionAction
{
    public class GetPermissionActionQuery : IGetPermissionActionQuery
    {
        private readonly RoleManager<AppRole> _roleManager;
        private readonly UserManager<AppUser> _userManager;
        private readonly IRepository<Permission, int> _permissionRepository;
        public GetPermissionActionQuery(RoleManager<AppRole> roleManager,
            IRepository<Permission, int> permissionRepository,
            UserManager<AppUser> userManager)
        {
            _roleManager = roleManager;
            _permissionRepository = permissionRepository;
            _userManager = userManager;
        }
        public async Task<bool> ExecuteAsync(string userId, string functionId, string action)
        {
            var user = await _userManager.FindByIdAsync(userId);
            var getRoles = await _userManager.GetRolesAsync(user);
            List<AppRoleViewModel> listRoleUser = new List<AppRoleViewModel>();
            if (getRoles.Count > 0)
            {
                foreach (var item in getRoles)
                {
                    var roleId = await _roleManager.Roles.Select(x => new AppRoleViewModel
                    {
                        Id = x.Id,
                        Description = x.Description,
                        Name = x.Name
                    }).Where(x => x.Name == item).FirstOrDefaultAsync();
                    listRoleUser.Add(roleId);
                }
            }
            bool isPermission = false;
            if (listRoleUser.Count > 0)
            {
                foreach (var item in listRoleUser)
                {
                    var getPermissions = await _permissionRepository.FindSingleAsync(x => x.RoleId == item.Id && x.FunctionId == functionId);
                    if(getPermissions != null)
                    {
                        if (action == ActionSetting.CanCreate)
                        {
                            if (getPermissions.CanCreate)
                            {
                                isPermission = true;
                            }
                        }

                        if (action == ActionSetting.CanUpdate)
                        {
                            if (getPermissions.CanUpdate)
                            {
                                isPermission = true;
                            }
                        }

                        if (action == ActionSetting.CanRead)
                        {
                            if (getPermissions.CanRead)
                            {
                                isPermission = true;
                            }
                        }

                        if (action == ActionSetting.CanDelete)
                        {
                            if (getPermissions.CanDelete)
                            {
                                isPermission = true;
                            }
                        }
                    }

                }

            }
            return isPermission;
        }

    }
}