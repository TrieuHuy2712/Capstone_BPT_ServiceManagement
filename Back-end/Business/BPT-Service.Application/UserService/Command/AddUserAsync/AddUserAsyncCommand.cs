using System;
using System.Threading.Tasks;
using BPT_Service.Application.UserService.ViewModel;
using BPT_Service.Model.Entities;
using Microsoft.AspNetCore.Identity;

namespace BPT_Service.Application.UserService.Command.AddUserAsync
{
    public class AddUserAsyncCommand : IAddUserAsyncCommand
    {
        private readonly UserManager<AppUser> _userManager;
        public AddUserAsyncCommand(UserManager<AppUser> userManager)
        {
            _userManager = userManager;
        }
        public async Task<bool> ExecuteAsync(AppUserViewModelinUserService userVm)
        {
            var user = new AppUser()
            {
                UserName = userVm.UserName,
                Avatar = userVm.Avatar,
                Email = userVm.Email,
                FullName = userVm.FullName,
                DateCreated = DateTime.Now,
                PhoneNumber = userVm.PhoneNumber
            };
            var result = await _userManager.CreateAsync(user, userVm.Password);
            if (result.Succeeded && userVm.Roles.Count > 0)
            {
                var appUser = await _userManager.FindByNameAsync(user.UserName);
                if (appUser != null)
                    await _userManager.AddToRolesAsync(appUser, userVm.Roles);
            }
            return true;
        }
    }
}