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
        private RoleManager<AppRole> _roleManager;
        private IRepository<Function, string> _functionRepository;
        private IRepository<Permission, int> _permissionRepository;
        private IRepository<Announcement, string> _announRepository;
        private IRepository<AnnouncementUser, int> _announUserRepository;
        private IUnitOfWork _unitOfWork;
        public RoleService(RoleManager<AppRole> roleManager,
            IUnitOfWork unitOfWork,
            IRepository<AnnouncementUser, int> announUserRepository,
         IRepository<Function, string> functionRepository,
         IRepository<Permission, int> permissionRepository,
            IRepository<Announcement, string> announRepository)
        {
            _unitOfWork = unitOfWork;
            _roleManager = roleManager;
            _announRepository = announRepository;
            _functionRepository = functionRepository;
            _announUserRepository = announUserRepository;
            _permissionRepository = permissionRepository;
        }

        public async Task<bool> AddAsync(AnnouncementViewModel announcementVm,
            List<AnnouncementUserViewModel> announcementUsers, AppRoleViewModel roleVm)
        {
            var role = new AppRole()
            {
                Name = roleVm.Name,
                Description = roleVm.Description
            };
            var result = await _roleManager.CreateAsync(role);
            Announcement announcement= new Announcement();
            announcement.Id= announcementVm.Id;
            announcementVm.Content = announcementVm.Content;
            announcementVm.DateCreated= announcementVm.DateCreated;
            announcementVm.DateModified = announcementVm.DateModified;
            announcementVm.Status = announcementVm.Status;
            announcementVm.Title = announcementVm.Title;
            announcementVm.UserId = announcementVm.UserId;


            _announRepository.Add(announcement);
            foreach (var userVm in announcementUsers)
            {
                AnnouncementUser user= new AnnouncementUser();
                user.Id = userVm.Id;
                user.UserId = userVm.UserId;
                user.AnnouncementId = userVm.AnnouncementId;
                user.HasRead = userVm.HasRead;
                _announUserRepository.Add(user);
            }
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
            return await _roleManager.Roles.Select(x=>new AppRoleViewModel{
                Id = x.Id,
                Description =x.Description,
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

            var data= query.Select(x=> new AppRoleViewModel{
                Name = x.Name,
                Id= x.Id,
                Description= x.Description
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

        public void SavePermission(List<PermissionViewModel> permissions, Guid roleId)
        {
            throw new NotImplementedException();
        }

        // public void SavePermission(List<PermissionViewModel> permissionVms, Guid roleId)
        // {
        //     var permissions = _mapper.Map<List<PermissionViewModel>, List<Permission>>(permissionVms);
        //     var oldPermission = _permissionRepository.FindAll().Where(x => x.RoleId == roleId).ToList();
        //     if (oldPermission.Count > 0)
        //     {
        //         _permissionRepository.RemoveMultiple(oldPermission);
        //     }
        //     foreach (var permission in permissions)
        //     {
        //         _permissionRepository.Add(permission);
        //     }
        //     _unitOfWork.Commit();
        // }

        public async Task UpdateAsync(AppRoleViewModel roleVm)
        {
            var role = await _roleManager.FindByIdAsync(roleVm.Id.ToString());
            role.Description = roleVm.Description;
            role.Name = roleVm.Name;
            await _roleManager.UpdateAsync(role);

        }
    }
}