using System.Threading.Tasks;
using BPT_Service.Application.RoleService.ViewModel;

namespace BPT_Service.Application.RoleService.Command.AddRoleAsync
{
    public interface IAddRoleAsyncCommand
    {
        Task<bool> ExecuteAync(AppRoleViewModel roleVm);
    }
}