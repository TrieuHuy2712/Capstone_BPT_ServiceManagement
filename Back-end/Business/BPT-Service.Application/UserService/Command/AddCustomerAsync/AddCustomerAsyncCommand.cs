using System;
using System.Threading.Tasks;
using BPT_Service.Application.UserService.ViewModel;
using BPT_Service.Model.Entities;
using BPT_Service.Model.Enums;
using Microsoft.AspNetCore.Identity;

namespace BPT_Service.Application.UserService.Command.AddCustomerAsync
{
    public class AddCustomerAsyncCommand : IAddCustomerAsyncCommand
    {
        private readonly UserManager<AppUser> _userManager;
         public AddCustomerAsyncCommand(UserManager<AppUser> userManager)
        {
            _userManager = userManager;
        }
        public async Task<bool> ExecuteAsync(AppUserViewModelinUserService userVm, string password)
        {
            var user = await _userManager.FindByNameAsync(userVm.UserName);
            if (user != null)
            {
                return false;
            }

            //Check exist email
            var email = await _userManager.FindByEmailAsync(userVm.Email);
            if (email != null)
            {
                return false;
            }

            if (user == null && email == null)
            {
                await _userManager.CreateAsync(new AppUser()
                {
                    UserName = userVm.UserName,
                    FullName = userVm.FullName,
                    Email = userVm.Email,
                    DateCreated = DateTime.Now,
                    DateModified = DateTime.Now,
                    Status = Status.Active
                }, password);
                var newUser = await _userManager.FindByNameAsync(userVm.UserName);
                await _userManager.AddToRoleAsync(user, "Customer");
            }
            return true;
        }
    }
}