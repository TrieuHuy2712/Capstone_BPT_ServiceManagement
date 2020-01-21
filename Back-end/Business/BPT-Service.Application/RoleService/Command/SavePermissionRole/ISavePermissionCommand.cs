using System.Threading.Tasks;
using BPT_Service.Application.RoleService.ViewModel;

namespace BPT_Service.Application.RoleService.Command.SavePermissionRole
{
    public interface ISavePermissionCommand
    {
       Task<bool> ExecuteAsync(RolePermissionViewModel rolePermissionViewModel);
    }
}