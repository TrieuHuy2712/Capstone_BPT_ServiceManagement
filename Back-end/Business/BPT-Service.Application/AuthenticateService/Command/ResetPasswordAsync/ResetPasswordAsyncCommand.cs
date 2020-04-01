using BPT_Service.Application.AuthenticateService.Command.ResetPasswordAsyncCommand;
using BPT_Service.Model.Entities;
using BPT_Service.WebAPI.Models.AccountViewModels;
using Microsoft.AspNetCore.Identity;
using System.Threading.Tasks;

namespace BPT_Service.Application.AuthenticateService.Command.ResetPasswordAsync
{
    public class ResetPasswordAsyncCommand : IResetPasswordAsyncCommand
    {
        private readonly UserManager<AppUser> _userManager;

        public ResetPasswordAsyncCommand(UserManager<AppUser> userManager)
        {
            _userManager = userManager;
        }

        public async Task<bool> ExecuteAsync(ChangePasswordViewModel model)
        {
            var user = await _userManager.FindByNameAsync(model.Username);
            if (user != null && await _userManager.CheckPasswordAsync(user, model.OldPassword))
            {
                string resetToken = await _userManager.GeneratePasswordResetTokenAsync(user);
                IdentityResult passwordChangeResult = await _userManager.ResetPasswordAsync(user, resetToken, model.NewPassword);
                if (passwordChangeResult.Succeeded)
                {
                    return true;
                }
            }
            return false;
        }
    }
}