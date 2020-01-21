using System.Threading.Tasks;
using BPT_Service.Application.UserService.ViewModel;

namespace BPT_Service.Application.UserService.Command.AddExternalAsync
{
    public interface IAddExternalAsyncCommand
    {
         Task<AppUserViewModelinUserService> ExecuteAsync(AppUserViewModelinUserService socialUserViewModel);
    }
}