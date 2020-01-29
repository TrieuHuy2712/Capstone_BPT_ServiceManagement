using System.Threading.Tasks;
using BPT_Service.Application.UserService.ViewModel;
using BPT_Service.Model.Entities;

namespace BPT_Service.Application.UserService.Command.DeleteUserAsync
{
    public interface IDeleteUserAsyncCommand
    {
         Task<CommandResult<AppUserViewModelinUserService>> ExecuteAsync(string id);
    }
}