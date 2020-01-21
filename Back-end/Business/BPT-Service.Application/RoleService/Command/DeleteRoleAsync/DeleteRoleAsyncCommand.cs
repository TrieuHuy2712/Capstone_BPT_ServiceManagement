using System;
using System.Threading.Tasks;
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
        public async Task ExecuteAsync(Guid id)
        {
            var role = await _roleManager.FindByIdAsync(id.ToString());
            await _roleManager.DeleteAsync(role);
        }
    }
}