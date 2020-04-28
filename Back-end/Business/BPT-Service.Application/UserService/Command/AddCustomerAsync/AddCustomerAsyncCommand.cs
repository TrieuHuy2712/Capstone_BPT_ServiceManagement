using BPT_Service.Application.UserService.ViewModel;
using BPT_Service.Common.Constants;
using BPT_Service.Common.Helpers;
using BPT_Service.Common.Logging;
using BPT_Service.Model.Entities;
using BPT_Service.Model.Enums;
using Microsoft.AspNetCore.Identity;
using System;
using System.Threading.Tasks;

namespace BPT_Service.Application.UserService.Command.AddCustomerAsync
{
    public class AddCustomerAsyncCommand : IAddCustomerAsyncCommand
    {
        private readonly UserManager<AppUser> _userManager;

        public AddCustomerAsyncCommand(UserManager<AppUser> userManager)
        {
            _userManager = userManager;
        }

        public async Task<CommandResult<AppUserViewModelinUserService>> ExecuteAsync(AppUserViewModelinUserService userVm)
        {
            try
            {
                var user = await _userManager.FindByNameAsync(userVm.UserName);
                if (user != null)
                {
                    return new CommandResult<AppUserViewModelinUserService>
                    {
                        isValid = false,
                        errorMessage = "Username has existed"
                    };
                }

                //Check exist email
                var email = await _userManager.FindByEmailAsync(userVm.Email);
                if (email != null)
                {
                    return new CommandResult<AppUserViewModelinUserService>
                    {
                        isValid = false,
                        errorMessage = "Email has existed"
                    };
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
                    }, userVm.Password);
                    var newUser = await _userManager.FindByNameAsync(userVm.UserName);
                    await _userManager.AddToRoleAsync(user, ConstantRoles.Customer);
                    await Logging<AddCustomerAsyncCommand>.
                        InformationAsync(ActionCommand.COMMAND_ADD, userVm.UserName, "New Account:" + userVm.UserName);
                    return new CommandResult<AppUserViewModelinUserService>
                    {
                        isValid = true,
                        myModel = new AppUserViewModelinUserService
                        {
                            UserName = userVm.UserName,
                            FullName = userVm.FullName,
                            Email = userVm.Email,
                            DateCreated = DateTime.Now,
                            Status = Status.Active
                        }
                    };
                }
                return new CommandResult<AppUserViewModelinUserService>
                {
                    isValid = false,
                    errorMessage = "Cannot create your new account"
                };
            }
            catch (System.Exception ex)
            {
                await Logging<AddCustomerAsyncCommand>.
                        ErrorAsync(ex, ActionCommand.COMMAND_ADD, userVm.UserName, "Has error");
                return new CommandResult<AppUserViewModelinUserService>
                {
                    isValid = false,
                    errorMessage = ex.InnerException.ToString()
                };
            }
        }
    }
}