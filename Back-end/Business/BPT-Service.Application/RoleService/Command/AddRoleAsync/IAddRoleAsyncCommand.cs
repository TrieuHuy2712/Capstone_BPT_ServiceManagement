using System.Threading.Tasks;
using BPT_Service.Application.RoleService.ViewModel;
using BPT_Service.Model.Entities;

namespace BPT_Service.Application.RoleService.Command.AddRoleAsync
{
    public interface IAddRoleAsyncCommand
    {
        Task<CommandResult<AppRoleViewModel>> ExecuteAync(AppRoleViewModel roleVm);
    }
}