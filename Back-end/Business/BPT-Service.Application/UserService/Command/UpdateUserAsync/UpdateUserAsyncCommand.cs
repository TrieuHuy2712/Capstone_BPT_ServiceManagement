using System;
using System.Linq;
using System.Threading.Tasks;
using BPT_Service.Application.UserService.ViewModel;
using BPT_Service.Model.Entities;
using Microsoft.AspNetCore.Identity;

namespace BPT_Service.Application.UserService.Command.UpdateUserAsync
{
    public class UpdateUserAsyncCommand : IUpdateUserAsyncCommand
    {
        private readonly UserManager<AppUser> _userManager;
        public UpdateUserAsyncCommand(UserManager<AppUser> userManager){
            _userManager = userManager;
        }
        public async Task<bool> ExecuteAsync(AppUserViewModelinUserService userVm)
        {
             try
            {
                var user = await _userManager.FindByIdAsync(userVm.Id.ToString());
                user.FullName = userVm.FullName;
                user.Status = userVm.Status;
                user.Email = userVm.Email;
                user.PhoneNumber = userVm.PhoneNumber;
                var userRoles = _userManager.GetRolesAsync(user);

                var selectedRole = userVm.NewRoles.ToArray();
                selectedRole = selectedRole ?? new string[] { };

                await _userManager.AddToRolesAsync(user, selectedRole.Except(userRoles.Result).ToArray());
                var userRoles1 = await _userManager.GetRolesAsync(user);
                await _userManager.UpdateAsync(user);
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return false;
            }
        }
    }
}