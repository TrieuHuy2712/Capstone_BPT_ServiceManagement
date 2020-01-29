using System.Threading.Tasks;
using BPT_Service.Application.UserService.ViewModel;
using BPT_Service.Model.Entities;

namespace BPT_Service.Application.UserService.Command.AddExternalAsync
{
    public interface IAddExternalAsyncCommand
    {
         Task<CommandResult<AppUserViewModelinUserService>> ExecuteAsync(AppUserViewModelinUserService socialUserViewModel);
    }
}