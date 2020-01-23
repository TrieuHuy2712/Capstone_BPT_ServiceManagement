using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BPT_Service.Application.UserService.ViewModel;
using BPT_Service.Common.Dtos;
using BPT_Service.Model.Entities;
using Microsoft.AspNetCore.Identity;

namespace BPT_Service.Application.UserService.Query.GetAllPagingAsync
{
    public class GetAllPagingUserAsyncQuery : IGetAllPagingUserAsyncQuery
    {
        private readonly UserManager<AppUser> _userManager;
        public GetAllPagingUserAsyncQuery(UserManager<AppUser> userManager)
        {
            _userManager = userManager;
        }
        public PagedResult<AppUserViewModelinUserService> ExecuteAsync(string keyword, int page, int pageSize)
        {
            var query = _userManager.Users;
            if (!string.IsNullOrEmpty(keyword))
                query = query.Where(x => x.FullName.Contains(keyword)
                || x.UserName.Contains(keyword)
                || x.Email.Contains(keyword));

            int totalRow = query.Count();
            query = query.Skip((page - 1) * pageSize)
               .Take(pageSize);

            var data = query.Select(x => new AppUserViewModelinUserService()
            {
                UserName = x.UserName,
                Avatar = x.Avatar,
                Email = x.Email,
                FullName = x.FullName,
                Id = x.Id,
                PhoneNumber = x.PhoneNumber,
                Status = x.Status,
                DateCreated = x.DateCreated
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