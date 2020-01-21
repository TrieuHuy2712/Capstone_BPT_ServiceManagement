using System.Threading.Tasks;
using BPT_Service.Application.UserService.ViewModel;

namespace BPT_Service.Application.UserService.Command.UpdateUserAsync
{
    public interface IUpdateUserAsyncCommand
    {
         Task<bool> ExecuteAsync(AppUserViewModelinUserService userVm);
    }
}