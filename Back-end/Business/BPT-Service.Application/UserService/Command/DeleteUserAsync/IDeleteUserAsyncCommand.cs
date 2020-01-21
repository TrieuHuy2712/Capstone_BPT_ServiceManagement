using System.Threading.Tasks;

namespace BPT_Service.Application.UserService.Command.DeleteUserAsync
{
    public interface IDeleteUserAsyncCommand
    {
         Task<bool> ExecuteAsync(string id);
    }
}