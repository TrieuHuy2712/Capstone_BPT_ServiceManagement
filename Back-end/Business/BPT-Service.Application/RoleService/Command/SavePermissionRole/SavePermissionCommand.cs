using System.Linq;
using System.Threading.Tasks;
using BPT_Service.Application.RoleService.ViewModel;
using BPT_Service.Model.Entities;
using BPT_Service.Model.Infrastructure.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace BPT_Service.Application.RoleService.Command.SavePermissionRole
{
    public class SavePermissionCommand : ISavePermissionCommand
    {
        private readonly IRepository<Permission, int> _permissionRepository;
        public SavePermissionCommand(IRepository<Permission, int> permissionRepository)
        {
            _permissionRepository = permissionRepository;
        }
        public async Task<bool> ExecuteAsync(RolePermissionViewModel rolePermissionViewModel)
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
                _permissionRepository.Add(permission);
            }
            await _permissionRepository.SaveAsync();
            return true;
        }
    }
}