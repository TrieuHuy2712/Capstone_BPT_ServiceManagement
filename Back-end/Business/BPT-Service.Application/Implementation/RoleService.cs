using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BPT_Service.Application.Interfaces;
using BPT_Service.Application.ViewModels.System;
using BPT_Service.Common.Dtos;
using BPT_Service.Model.Entities;
using BPT_Service.Model.Infrastructure.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace BPT_Service.Application.Implementation
{
    public class RoleService : IRoleService
    {
        #region  Constuctor
        private readonly RoleManager<AppRole> _roleManager;
        private readonly UserManager<AppUser> _userManager;
        private readonly IRepository<Function, string> _functionRepository;
        private readonly IRepository<Permission, int> _permissionRepository;
        private  readonly IUnitOfWork _unitOfWork;
        public RoleService(RoleManager<AppRole> roleManager,
            IUnitOfWork unitOfWork,
            IRepository<Function, string> functionRepository,
            IRepository<Permission, int> permissionRepository,
            UserManager<AppUser> userManager)
        {
            _unitOfWork = unitOfWork;
            _roleManager = roleManager;
            _functionRepository = functionRepository;
            _permissionRepository = permissionRepository;
            _userManager = userManager;
        }
        #endregion

        public async Task<bool> AddAsync(AppRoleViewModel roleVm)
        {
            var role = new AppRole()
            {
                Name = roleVm.Name,
                Description = roleVm.Description
            };
            var result = await _roleManager.CreateAsync(role);
            _unitOfWork.Commit();
            return result.Succeeded;
        }

        public Task<bool> CheckPermission(string functionId, string action, string[] roles)
        {
            var functions = _functionRepository.FindAll();
            var permissions = _permissionRepository.FindAll();
            var query = from f in functions
                        join p in permissions on f.Id equals p.FunctionId
                        join r in _roleManager.Roles on p.RoleId equals r.Id
                        where roles.Contains(r.Name) && f.Id == functionId
                        && ((p.CanCreate && action == "Create")
                        || (p.CanUpdate && action == "Update")
                        || (p.CanDelete && action == "Delete")
                        || (p.CanRead && action == "Read"))
                        select p;
            return query.AnyAsync();
        }

        public async Task DeleteAsync(Guid id)
        {
            var role = await _roleManager.FindByIdAsync(id.ToString());
            await _roleManager.DeleteAsync(role);
        }

        public async Task<List<AppRoleViewModel>> GetAllAsync()
        {
            return await _roleManager.Roles.Select(x => new AppRoleViewModel
            {
                Id = x.Id,
                Description = x.Description,
                NameVietNamese =x.NameVietNamese,
                Name = x.Name
            }).ToListAsync();
        }

        public PagedResult<AppRoleViewModel> GetAllPagingAsync(string keyword, int page, int pageSize)
        {
            var query = _roleManager.Roles;
            if (!string.IsNullOrEmpty(keyword))
                query = query.Where(x => x.Name.Contains(keyword)
                || x.Description.Contains(keyword));

            int totalRow = query.Count();
            query = query.Skip((page - 1) * pageSize)
               .Take(pageSize);

            var data = query.Select(x => new AppRoleViewModel
            {
                Name = x.Name,
                NameVietNamese =x.NameVietNamese,
                Id = x.Id,
                Description = x.Description
            }).ToList();

            var paginationSet = new PagedResult<AppRoleViewModel>()
            {
                Results = data,
                CurrentPage = page,
                RowCount = totalRow,
                PageSize = pageSize
            };

            return paginationSet;
        }

        public async Task<AppRoleViewModel> GetById(Guid id)
        {
            var role = await _roleManager.FindByIdAsync(id.ToString());
            //return _mapper.Map<AppRole, AppRoleViewModel>(role);
            AppRoleViewModel roleViewModel = new AppRoleViewModel();
            roleViewModel.Id = role.Id;
            roleViewModel.Name = role.Name;
            roleViewModel.Description = role.Description;
            return roleViewModel;
        }

        public List<PermissionViewModel> GetListFunctionWithRole(Guid roleId)
        {
            var functions = _functionRepository.FindAll();
            var permissions = _permissionRepository.FindAll();

            var query = from f in functions
                        join p in permissions on f.Id equals p.FunctionId into fp
                        from p in fp.DefaultIfEmpty()
                        where p != null && p.RoleId == roleId
                        select new PermissionViewModel()
                        {
                            RoleId = roleId,
                            FunctionId = f.Id,
                            CanCreate = p != null ? p.CanCreate : false,
                            CanDelete = p != null ? p.CanDelete : false,
                            CanRead = p != null ? p.CanRead : false,
                            CanUpdate = p != null ? p.CanUpdate : false
                        };
            return query.ToList();
        }

        public List<PermissionViewModel> GetAllPermission(string functionId)
        {
            List<PermissionViewModel> permissions = new List<PermissionViewModel>();
            var roles = _roleManager.Roles.Where(x => x.Name != "Admin").ToList();
            var getPermission = _permissionRepository.FindAll();
            var listPermission = _permissionRepository.FindAll().Where(x => x.FunctionId == functionId).ToList();
            if (listPermission.Count == 0)
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

        public void SavePermission(RolePermissionViewModel rolePermissionViewModel)
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
            var oldPermission = _permissionRepository.FindAll().Where(x => x.FunctionId == rolePermissionViewModel.FunctionId).ToList();
            if (oldPermission.Count > 0)
            {
                _permissionRepository.RemoveMultiple(oldPermission);
            }
            foreach (var permission in permissions)
            {
                _permissionRepository.Add(permission);
            }
            _unitOfWork.Commit();
        }

        public async Task UpdateAsync(AppRoleViewModel roleVm)
        {
            var role = await _roleManager.FindByIdAsync(roleVm.Id.ToString());
            role.Description = roleVm.Description;
            role.Name = roleVm.Name;
            await _roleManager.UpdateAsync(role);

        }
    }
}