using System;
using System.Threading.Tasks;
using BPT_Service.Application.RoleService.ViewModel;
using BPT_Service.Model.Entities;
using Microsoft.AspNetCore.Identity;

namespace BPT_Service.Application.RoleService.Command.DeleteRoleAsync
{
    public class DeleteRoleAsyncCommand : IDeleteRoleAsyncCommand
    {
        private readonly RoleManager<AppRole> _roleManager;
        public DeleteRoleAsyncCommand(RoleManager<AppRole> roleManager)
        {
            _roleManager = roleManager;
        }
        public async Task<CommandResult<AppRoleViewModel>> ExecuteAsync(Guid id)
        {
            try
            {
                var role = await _roleManager.FindByIdAsync(id.ToString());
                if (role != null)
                {
                    await _roleManager.DeleteAsync(role);
                    return new CommandResult<AppRoleViewModel>
                    {
                        isValid = true
                    };
                }
                return new CommandResult<AppRoleViewModel>
                {
                    isValid = false
                };
            }
            catch (System.Exception ex)
            {
                return new CommandResult<AppRoleViewModel>
                {
                    isValid = false,
                    errorMessage = ex.InnerException.ToString()
                };
            }
        }
    }
}