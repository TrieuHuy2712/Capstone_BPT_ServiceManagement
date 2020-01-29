using System.Threading.Tasks;
using BPT_Service.Application.UserService.ViewModel;
using BPT_Service.Model.Entities;

namespace BPT_Service.Application.UserService.Command.UpdateUserAsync
{
    public interface IUpdateUserAsyncCommand
    {
         Task<CommandResult<AppUserViewModelinUserService>> ExecuteAsync(AppUserViewModelinUserService userVm);
    }
}