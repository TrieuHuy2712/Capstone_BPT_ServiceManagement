using System.Threading.Tasks;
using BPT_Service.Application.RoleService.ViewModel;

namespace BPT_Service.Application.PermissionService.Query.GetPermissionRoleQuery
{
    public interface IGetPermissionRoleQuery
    {
         Task<PermissionSingleViewModel> ExecuteAsync(string userName, string functionId);
    }
}