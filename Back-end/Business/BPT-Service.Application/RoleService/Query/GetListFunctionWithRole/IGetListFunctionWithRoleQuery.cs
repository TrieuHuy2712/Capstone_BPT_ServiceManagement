using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using BPT_Service.Application.RoleService.ViewModel;

namespace BPT_Service.Application.RoleService.Query.GetListFunctionWithRole
{
    public interface IGetListFunctionWithRoleQuery
    {
         Task<List<PermissionViewModel>> ExecuteAsync(Guid roleId);
    }
}