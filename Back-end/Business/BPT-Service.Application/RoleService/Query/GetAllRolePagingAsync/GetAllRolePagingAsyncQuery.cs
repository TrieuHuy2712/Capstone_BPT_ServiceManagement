using System.Linq;
using BPT_Service.Application.RoleService.ViewModel;
using BPT_Service.Common.Dtos;
using BPT_Service.Model.Entities;
using Microsoft.AspNetCore.Identity;

namespace BPT_Service.Application.RoleService.Query.GetAllPagingAsync
{
    public class GetAllRolePagingAsyncQuery : IGetAllRolePagingAsyncQuery
    {
        private readonly RoleManager<AppRole> _roleManager;
        public GetAllRolePagingAsyncQuery(RoleManager<AppRole> roleManager)
        {
            _roleManager = roleManager;
        }

        public PagedResult<AppRoleViewModel> ExecuteAsync(string keyword, int page, int pageSize)
        {
            var query = _roleManager.Roles;
            if (!string.IsNullOrEmpty(keyword))
                query = query.Where(x => x.Name.Contains(keyword)
                || x.Description.Contains(keyword));

            int totalRow = query.Count();
            query = query.Skip((page - 1) * pageSize)
               .Take(pageSize);

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