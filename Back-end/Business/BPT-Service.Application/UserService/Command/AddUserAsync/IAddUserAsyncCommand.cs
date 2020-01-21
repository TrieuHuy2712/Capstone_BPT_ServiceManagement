using System.Threading.Tasks;
using BPT_Service.Application.UserService.ViewModel;

namespace BPT_Service.Application.UserService.Command.AddUserAsync
{
    public interface IAddUserAsyncCommand
    {
        Task<bool> ExecuteAsync(AppUserViewModelinUserService userVm);
    }
}