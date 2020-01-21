using System;
using System.Threading.Tasks;
using BPT_Service.Application.RoleService.ViewModel;
using BPT_Service.Model.Entities;
using Microsoft.AspNetCore.Identity;

namespace BPT_Service.Application.RoleService.Query.GetByIdAsync
{
    public class GetRoleByIdAsyncQuery : IGetRoleByIdAsyncQuery
    {
        private readonly RoleManager<AppRole> _roleManager;
        public GetRoleByIdAsyncQuery(RoleManager<AppRole> roleManager)
        {
            _roleManager = roleManager;
        }
        public async Task<AppRoleViewModel> ExecuteAsync(Guid id)
        {
            var role = await _roleManager.FindByIdAsync(id.ToString());
            AppRoleViewModel roleViewModel = new AppRoleViewModel();
            roleViewModel.Id = role.Id;
            roleViewModel.Name = role.Name;
            roleViewModel.Description = role.Description;
            return roleViewModel;
        }
    }
}