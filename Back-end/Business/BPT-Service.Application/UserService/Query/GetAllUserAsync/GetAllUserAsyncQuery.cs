using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BPT_Service.Application.UserService.ViewModel;
using BPT_Service.Model.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace BPT_Service.Application.UserService.Query.GetAllAsync
{
    public class GetAllUserAsyncQuery : IGetAllUserAsyncQuery
    {
        private readonly UserManager<AppUser> _userManager;
        public GetAllUserAsyncQuery(UserManager<AppUser> userManager)
        {
            _userManager = userManager;
        }
        public async Task<List<AppUserViewModelinUserService>> ExecuteAsync()
        {
            return await _userManager.Users.Select(x => new AppUserViewModelinUserService()
            {
                Avatar = x.Avatar,
                DateCreated = x.DateCreated,
                Email = x.Email,
                FullName = x.FullName,
                PhoneNumber = x.PhoneNumber,
                Token = x.Token,
                UserName = x.UserName,
            }).ToListAsync();
        }
    }
}