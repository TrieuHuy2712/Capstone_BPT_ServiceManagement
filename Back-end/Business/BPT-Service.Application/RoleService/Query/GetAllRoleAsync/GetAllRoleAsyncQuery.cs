using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BPT_Service.Application.RoleService.ViewModel;
using BPT_Service.Model.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace BPT_Service.Application.RoleService.Query.GetAllAsync
{
    public class GetAllRoleAsyncQuery : IGetAllRoleAsyncQuery
    {
        private readonly RoleManager<AppRole> _roleManager;
        public GetAllRoleAsyncQuery(RoleManager<AppRole> roleManager)
        {
            _roleManager = roleManager;
        }
        public async Task<List<AppRoleViewModel>> ExecuteAsync()
        {
            return await _roleManager.Roles.Select(x => new AppRoleViewModel
            {
                Id = x.Id,
                Description = x.Description,
                NameVietNamese = x.NameVietNamese,
                Name = x.Name
            }).ToListAsync();
        }
    }
}