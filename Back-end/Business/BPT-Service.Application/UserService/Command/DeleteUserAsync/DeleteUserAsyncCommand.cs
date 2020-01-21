using System.Threading.Tasks;
using BPT_Service.Model.Entities;
using Microsoft.AspNetCore.Identity;

namespace BPT_Service.Application.UserService.Command.DeleteUserAsync
{
    public class DeleteUserAsyncCommand : IDeleteUserAsyncCommand
    {
        private readonly UserManager<AppUser> _userManager;
        public DeleteUserAsyncCommand(UserManager<AppUser> userManager)
        {
            _userManager = userManager;
        }
        public async Task<bool> ExecuteAsync(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user != null)
            {
                await _userManager.DeleteAsync(user);
                return true;
            }
            return false;
        }
    }
}