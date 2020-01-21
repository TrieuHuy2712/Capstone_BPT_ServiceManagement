using System.Threading.Tasks;

namespace BPT_Service.Application.AuthenticateService.Command.ResetPasswordAsyncCommand
{
    public interface IResetPasswordAsyncCommand
    {
         Task<bool> ExecuteAsync(string username, string oldPassword, string newPassword);
    }
}