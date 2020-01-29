using System.Threading.Tasks;
using BPT_Service.Application.RoleService.ViewModel;
using BPT_Service.Model.Entities;

namespace BPT_Service.Application.RoleService.Command.SavePermissionRole
{
    public interface ISavePermissionCommand
    {
        Task<CommandResult<RolePermissionViewModel>> ExecuteAsync(RolePermissionViewModel rolePermissionViewModel);
    }
}