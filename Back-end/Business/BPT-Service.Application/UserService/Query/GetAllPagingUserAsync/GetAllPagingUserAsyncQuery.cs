using BPT_Service.Application.UserService.ViewModel;
using BPT_Service.Common.Dtos;
using BPT_Service.Common.Support;
using BPT_Service.Model.Entities;
using Microsoft.AspNetCore.Identity;
using System.Linq;
using System.Threading.Tasks;

namespace BPT_Service.Application.UserService.Query.GetAllPagingAsync
{
    public class GetAllPagingUserAsyncQuery : IGetAllPagingUserAsyncQuery
    {
        private readonly UserManager<AppUser> _userManager;

        public GetAllPagingUserAsyncQuery(UserManager<AppUser> userManager)
        {
            _userManager = userManager;
        }

        public async Task<PagedResult<AppUserViewModelinUserService>> ExecuteAsync(string keyword, int page, int pageSize)
        {
            var query = _userManager.Users.ToList();
            if (!string.IsNullOrEmpty(keyword))
            {
                query = query.Where(x => x.FullName.ToLower().Contains(keyword.ToLower())
                || LevenshteinDistance.Compute(x.FullName.ToLower(), keyword.ToLower()) <= 3
                || x.UserName.ToLower().Contains(keyword.ToLower())
                || LevenshteinDistance.Compute(x.UserName.ToLower(), keyword.ToLower()) <= 3
                || x.Email.ToLower().Contains(keyword.ToLower())
                || LevenshteinDistance.Compute(x.Email.ToLower(), keyword.ToLower()) <= 3
                || x.PhoneNumber.ToLower().Contains(keyword.ToLower())
                || LevenshteinDistance.Compute(x.PhoneNumber.ToLower(), keyword.ToLower()) <= 3
                ).ToList();
            }

            int totalRow = query.Count();
            if (pageSize != 0)
            {
                query = query.Skip((page - 1) * pageSize)
                   .Take(pageSize).ToList();
            }

            var data = query.Select(x => new AppUserViewModelinUserService()
            {
                UserName = x.UserName,
                Avatar = x.Avatar,
                Email = x.Email,
                FullName = x.FullName,
                Id = x.Id,
                PhoneNumber = x.PhoneNumber,
                Status = x.Status,
                DateCreated = x.DateCreated,
                Roles = _userManager.GetRolesAsync(x).Result.ToList()
            }).ToList();
            var paginationSet = new PagedResult<AppUserViewModelinUserService>()
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