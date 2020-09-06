using BPT_Service.Application.RoleService.ViewModel;
using BPT_Service.Common.Dtos;
using BPT_Service.Common.Support;
using BPT_Service.Model.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

namespace BPT_Service.Application.RoleService.Query.GetAllPagingAsync
{
    public class GetAllRolePagingAsyncQuery : IGetAllRolePagingAsyncQuery
    {
        private readonly RoleManager<AppRole> _roleManager;

        public GetAllRolePagingAsyncQuery(
            RoleManager<AppRole> roleManager)
        {
            _roleManager = roleManager;
        }

        public async Task<PagedResult<AppRoleViewModel>> ExecuteAsync(string keyword, int page, int pageSize)
        {
            
            var query = await _roleManager.Roles.ToListAsync();
            if (!string.IsNullOrEmpty(keyword))
                query = query.Where(x => x.Name.ToLower().Contains(keyword.ToLower())
                || LevenshteinDistance.Compute(x.Name.ToLower(), keyword.ToLower()) <= 3
                || LevenshteinDistance.Compute(x.Description.ToLower(), keyword.ToLower()) <= 3
                || x.Description.ToLower().Contains(keyword.ToLower())).ToList();

            int totalRow = query.Count();
            if (pageSize != 0)
            {
                query = query.Skip((page - 1) * pageSize)
                   .Take(pageSize).ToList();
            }

            var data = query.Select(x => new AppRoleViewModel
            {
                Name = x.Name,
                Id = x.Id,
                Description = x.Description
            }).ToList();

            var paginationSet = new PagedResult<AppRoleViewModel>()
            {
                Results = data,
                CurrentPage = page,
                RowCount = totalRow,
                PageSize = pageSize
            };

            return paginationSet;
        }
    }
}