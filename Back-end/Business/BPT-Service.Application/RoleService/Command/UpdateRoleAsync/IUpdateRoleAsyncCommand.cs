using System.Threading.Tasks;
using BPT_Service.Application.RoleService.ViewModel;

namespace BPT_Service.Application.RoleService.Command.UpdateRoleAsync
{
    public interface IUpdateRoleAsyncCommand
    {
        Task ExecuteAsync(AppRoleViewModel roleVm);
    }
}