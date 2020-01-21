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
        public async Task ExecuteAsync(AppRoleViewModel roleVm)
        {
            var role = await _roleManager.FindByIdAsync(roleVm.Id.ToString());
            role.Description = roleVm.Description;
            role.Name = roleVm.Name;
            await _roleManager.UpdateAsync(role);
        }
    }
}