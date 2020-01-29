using System.Threading.Tasks;
using BPT_Service.Application.RoleService.ViewModel;
using BPT_Service.Model.Entities;

namespace BPT_Service.Application.RoleService.Command.UpdateRoleAsync
{
    public interface IUpdateRoleAsyncCommand
    {
        Task<CommandResult<AppRoleViewModel>> ExecuteAsync(AppRoleViewModel roleVm);
    }
}