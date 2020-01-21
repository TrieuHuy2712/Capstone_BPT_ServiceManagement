using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BPT_Service.Application.RoleService.ViewModel;
using BPT_Service.Model.Entities;
using BPT_Service.Model.Infrastructure.Interfaces;

namespace BPT_Service.Application.RoleService.Query.GetListFunctionWithRole
{
    public class GetListFunctionWithRoleQuery : IGetListFunctionWithRoleQuery
    {
        private readonly IRepository<Function, string> _functionRepository;
        private readonly IRepository<Permission, int> _permissionRepository;
        public GetListFunctionWithRoleQuery(IRepository<Function, string> functionRepository,
        IRepository<Permission, int> permissionRepository)
        {
            _functionRepository = functionRepository;
            _permissionRepository = permissionRepository;
        }
        public async Task<List<PermissionViewModel>> ExecuteAsync(Guid roleId)
        {
            var functions = await _functionRepository.FindAllAsync();
            var permissions = await _permissionRepository.FindAllAsync();

            var query = (from f in functions
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
                        });
            return query.ToList();
        }

    }
}