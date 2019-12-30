using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BPT_Service.Application.Interfaces;
using BPT_Service.Application.ViewModels.System;
using BPT_Service.Model.Entities;
using BPT_Service.Model.Infrastructure.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace BPT_Service.Application.Implementation
{
    public class PermissionService : IPermissionService
    {
        #region Constructor
        private readonly RoleManager<AppRole> _roleManager;
        private readonly UserManager<AppUser> _userManager;
        private readonly IRepository<Permission, int> _permissionRepository;
        public PermissionService(RoleManager<AppRole> roleManager,
            IRepository<Permission, int> permissionRepository,
            UserManager<AppUser> userManager)
        {
            _roleManager = roleManager;
            _permissionRepository = permissionRepository;
            _userManager = userManager;
        }
        #endregion
        public async Task<PermissionSingleViewModel> GetPermissionRole(string userName, string functionId)
        {
            List<Guid> listIdRole = new List<Guid>();
            List<PermissionSingleViewModel> permissionSingleVM = new List<PermissionSingleViewModel>();
            var userRoleIds = _roleManager.Roles.Select(x => new
            {
                Id = x.Id,
                Name = x.Name,

            }).ToList();
            var user = await _userManager.FindByNameAsync(userName);
            var roles = await _userManager.GetRolesAsync(user);

            List<AppRoleViewModel> listRoleUser = new List<AppRoleViewModel>();
            if (roles.Count > 0)
            {
                foreach (var item in roles)
                {
                    var roleId = await _roleManager.Roles.Select(x => new AppRoleViewModel
                    {
                        Id = x.Id,
                        Description = x.Description,
                        NameVietNamese = x.NameVietNamese,
                        Name = x.Name
                    }).Where(x => x.Name == item).FirstOrDefaultAsync();
                    listRoleUser.Add(roleId);
                }
            }
            if (listRoleUser.Count > 0)
            {
                foreach (var item in listRoleUser)
                {
                    var getPermissions = _permissionRepository.FindAll()
                                        .Where(x => x.RoleId == item.Id && x.FunctionId == functionId)
                                        .FirstOrDefault();
                    if (getPermissions != null)
                    {
                        PermissionSingleViewModel tempPermission = new PermissionSingleViewModel();
                        tempPermission.Id = getPermissions.Id;
                        tempPermission.RoleId = getPermissions.RoleId;
                        tempPermission.FunctionId = getPermissions.FunctionId;
                        tempPermission.CanRead = getPermissions.CanRead;
                        tempPermission.CanCreate = getPermissions.CanCreate;
                        tempPermission.CanDelete = getPermissions.CanDelete;
                        tempPermission.CanUpdate = getPermissions.CanUpdate;
                        permissionSingleVM.Add(tempPermission);
                    }
                }

            }
            return GetDuplicate(permissionSingleVM);
        }
        private PermissionSingleViewModel GetDuplicate(List<PermissionSingleViewModel> permissionSingles)
        {
            PermissionSingleViewModel permission = new PermissionSingleViewModel();
            var getlistPerFunction = permissionSingles.ToList();
            if (getlistPerFunction.Count() > 1)
            {
                for (int i = 0; i < getlistPerFunction.Count - 1; i++)
                {
                    //Check case Can Create
                    if (getlistPerFunction[i].CanCreate != getlistPerFunction[i + 1].CanCreate
                        && (getlistPerFunction[i].CanCreate == true || getlistPerFunction[i + 1].CanCreate == true))
                    {
                        permission.CanCreate = true;
                    }
                    else if (getlistPerFunction[i].CanCreate == getlistPerFunction[i + 1].CanCreate
                       && getlistPerFunction[i].CanCreate == true)
                    {
                        permission.CanCreate = true;
                    }
                    else
                    {
                        permission.CanCreate = false;
                    }

                    //Check case can Read
                    if (getlistPerFunction[i].CanRead != getlistPerFunction[i + 1].CanRead
                        && (getlistPerFunction[i].CanRead == true || getlistPerFunction[i + 1].CanRead == true))
                    {
                        permission.CanRead = true;
                    }
                    else if (getlistPerFunction[i].CanRead == getlistPerFunction[i + 1].CanRead
                       && getlistPerFunction[i].CanRead == true)
                    {
                        permission.CanRead = true;
                    }
                    else
                    {
                        permission.CanRead = false;
                    }

                    //Check case can Update
                    if (getlistPerFunction[i].CanUpdate != getlistPerFunction[i + 1].CanUpdate
                        && (getlistPerFunction[i].CanUpdate == true || getlistPerFunction[i + 1].CanUpdate == true))
                    {
                        permission.CanUpdate = true;
                    }
                    else if (getlistPerFunction[i].CanUpdate == getlistPerFunction[i + 1].CanUpdate
                       && getlistPerFunction[i].CanUpdate == true)
                    {
                        permission.CanUpdate = true;
                    }
                    else
                    {
                        permission.CanUpdate = false;
                    }

                    //Check case can Delete
                    if (getlistPerFunction[i].CanDelete != getlistPerFunction[i + 1].CanDelete
                        && (getlistPerFunction[i].CanDelete == true || getlistPerFunction[i + 1].CanDelete == true))
                    {
                        permission.CanDelete = true;
                    }
                    else if (getlistPerFunction[i].CanDelete == getlistPerFunction[i + 1].CanDelete
                       && getlistPerFunction[i].CanDelete == true)
                    {
                        permission.CanDelete = true;
                    }
                    else
                    {
                        permission.CanDelete = false;
                    }
                }
            }
            else
            {
                return permissionSingles.FirstOrDefault();
            }
            return permission;
        }
    }
}