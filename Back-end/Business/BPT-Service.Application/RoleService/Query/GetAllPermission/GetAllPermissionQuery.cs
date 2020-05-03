using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BPT_Service.Application.RoleService.ViewModel;
using BPT_Service.Common.Constants;
using BPT_Service.Model.Entities;
using BPT_Service.Model.Infrastructure.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace BPT_Service.Application.RoleService.Query.GetAllPermission
{
    public class GetAllPermissionQuery : IGetAllPermissionQuery
    {
        private readonly RoleManager<AppRole> _roleManager;
        private readonly IRepository<Permission, int> _permissionRepository;
        public GetAllPermissionQuery(RoleManager<AppRole> roleManager,IRepository<Permission, int> permissionRepository){
            _roleManager = roleManager;
            _permissionRepository = permissionRepository;
        }
        public async Task<List<PermissionViewModel>> ExecuteAsync(string functionId)
        {
            List<PermissionViewModel> permissions = new List<PermissionViewModel>();
            var roles = await _roleManager.Roles.Where(x => x.Name != ConstantRoles.Admin 
            || x.Name != ConstantRoles.Provider
            || x.Name != ConstantRoles.Customer).ToListAsync();
            var getPermission = await _permissionRepository.FindAllAsync();
            var listPermission = await _permissionRepository.FindAllAsync(x => x.FunctionId == functionId);
            if (listPermission.Count() == 0)
            {
                foreach (var item in roles)
                {
                    permissions.Add(new PermissionViewModel()
                    {
                        RoleId = item.Id,
                        CanCreate = false,
                        CanDelete = false,
                        CanRead = false,
                        CanUpdate = false,
                        RoleName = item.Name
                    });
                }
            }
            else
            {
                foreach (var item in roles)
                {
                    if (!listPermission.Any(x => x.RoleId == item.Id))
                    {
                        permissions.Add(new PermissionViewModel()
                        {
                            RoleId = item.Id,
                            CanCreate = false,
                            CanDelete = false,
                            CanRead = false,
                            CanUpdate = false,
                            RoleName = item.Name
                        });
                    }
                }
                var getDetailFunction = (from r in roles
                                         join p in getPermission
                                         on r.Id equals p.RoleId
                                         where p.FunctionId == functionId
                                         select new PermissionViewModel
                                         {
                                             RoleId = r.Id,
                                             CanCreate = p.CanCreate,
                                             CanDelete = p.CanDelete,
                                             CanRead = p.CanRead,
                                             CanUpdate = p.CanUpdate,
                                             RoleName = r.Name
                                         }).ToList();
                permissions.AddRange(getDetailFunction);
            }
            return permissions.ToList();
        }

    }
}