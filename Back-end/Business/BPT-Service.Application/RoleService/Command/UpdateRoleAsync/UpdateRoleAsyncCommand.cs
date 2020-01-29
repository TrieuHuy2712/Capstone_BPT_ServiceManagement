using System.Threading.Tasks;
using BPT_Service.Application.RoleService.ViewModel;
using BPT_Service.Model.Entities;
using Microsoft.AspNetCore.Identity;

namespace BPT_Service.Application.RoleService.Command.UpdateRoleAsync
{
    public class UpdateRoleAsyncCommand : IUpdateRoleAsyncCommand
    {
        private readonly RoleManager<AppRole> _roleManager;
        public UpdateRoleAsyncCommand(RoleManager<AppRole> roleManager)
        {
            _roleManager = roleManager;
        }
        public async Task<CommandResult<AppRoleViewModel>> ExecuteAsync(AppRoleViewModel roleVm)
        {
            try
            {
                var role = await _roleManager.FindByIdAsync(roleVm.Id.ToString());
                if (role != null)
                {
                    role.Description = roleVm.Description;
                    role.Name = roleVm.Name;
                    await _roleManager.UpdateAsync(role);
                    return new CommandResult<AppRoleViewModel>
                    {
                        isValid = true,
                        myModel = new AppRoleViewModel
                        {
                            Description = role.Description,
                            Id = role.Id,
                            Name = role.Name
                        }
                    };
                }
                return new CommandResult<AppRoleViewModel>
                {
                    isValid = false,
                    errorMessage = "Cannot find Id Role"
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