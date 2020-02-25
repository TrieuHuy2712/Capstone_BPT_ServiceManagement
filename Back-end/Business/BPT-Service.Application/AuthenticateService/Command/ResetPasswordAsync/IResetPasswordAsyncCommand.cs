using System.Threading.Tasks;
using BPT_Service.WebAPI.Models.AccountViewModels;

namespace BPT_Service.Application.AuthenticateService.Command.ResetPasswordAsyncCommand
{
    public interface IResetPasswordAsyncCommand
    {
         Task<bool> ExecuteAsync(ChangePasswordViewModel model);
    }
}