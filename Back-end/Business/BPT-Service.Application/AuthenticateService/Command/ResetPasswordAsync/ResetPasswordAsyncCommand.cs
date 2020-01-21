using System.Threading.Tasks;
using BPT_Service.Application.AuthenticateService.Command.ResetPasswordAsyncCommand;
using BPT_Service.Model.Entities;
using Microsoft.AspNetCore.Identity;

namespace BPT_Service.Application.AuthenticateService.Command.ResetPasswordAsync
{
    public class ResetPasswordAsyncCommand : IResetPasswordAsyncCommand
    {
         private readonly UserManager<AppUser> _userManager;

        public ResetPasswordAsyncCommand(UserManager<AppUser> userManager)
        {
            _userManager = userManager;

        }
        public async Task<bool> ExecuteAsync(string username, string oldPassword, string newPassword)
        {
             var user = await _userManager.FindByNameAsync(username);
            if (user != null && await _userManager.CheckPasswordAsync(user, oldPassword))
            {
                string resetToken = await _userManager.GeneratePasswordResetTokenAsync(user);
                IdentityResult passwordChangeResult = await _userManager.ResetPasswordAsync(user, resetToken, newPassword);
                if (passwordChangeResult.Succeeded)
                {
                    return true;
                }
            }
            return false;
        }
    }
}