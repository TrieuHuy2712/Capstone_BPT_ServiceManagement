using System.Threading.Tasks;
using BPT_Service.Application.UserService.ViewModel;
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
        public async Task<CommandResult<AppUserViewModelinUserService>> ExecuteAsync(string id)
        {
            try
            {
                var user = await _userManager.FindByIdAsync(id);
                if (user != null)
                {
                    await _userManager.DeleteAsync(user);
                    return new CommandResult<AppUserViewModelinUserService>
                    {
                        isValid = true,
                        myModel = new AppUserViewModelinUserService
                        {
                            UserName = user.UserName,
                            Avatar = user.Avatar,
                            Email = user.Email,
                            FullName = user.FullName,
                            DateCreated = user.DateCreated,
                        }
                    };
                }
                return new CommandResult<AppUserViewModelinUserService>
                {
                    isValid = false,
                    errorMessage = " Cannot not find Id User"
                };
            }
            catch (System.Exception ex)
            {
                return new CommandResult<AppUserViewModelinUserService>
                {
                    isValid = false,
                    errorMessage = ex.InnerException.ToString()
                };
            }
        }
    }
}