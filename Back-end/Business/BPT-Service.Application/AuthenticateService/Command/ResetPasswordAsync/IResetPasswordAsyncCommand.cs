using BPT_Service.WebAPI.Models.AccountViewModels;
using System.Threading.Tasks;

namespace BPT_Service.Application.AuthenticateService.Command.ResetPasswordAsyncCommand
{
    public interface IResetPasswordAsyncCommand
    {
        Task<bool> ExecuteAsync(ChangePasswordViewModel model);
    }
}