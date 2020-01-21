using System.Collections.Generic;
using System.Threading.Tasks;
using BPT_Service.Application.RoleService.ViewModel;

namespace BPT_Service.Application.RoleService.Query.GetAllPermission
{
    public interface IGetAllPermissionQuery
    {
         Task<List<PermissionViewModel>> ExecuteAsync(string functionId);
    }
}