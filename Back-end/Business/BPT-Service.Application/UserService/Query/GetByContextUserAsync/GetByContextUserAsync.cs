using BPT_Service.Application.UserService.ViewModel;
using BPT_Service.Model.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace BPT_Service.Application.UserService.Query.GetByContextUserAsync
{
    public class GetByContextUserAsync : IGetByContextUserAsync
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public GetByContextUserAsync(
            UserManager<AppUser> userManager,
            IHttpContextAccessor httpContextAccessor)
        {
            _userManager = userManager;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<AppUserViewModelinUserService> ExcecuteAsync()
        {
            var userId = _httpContextAccessor.HttpContext.User.Identity.Name;
            var findUser = await _userManager.FindByIdAsync(userId);
            var roles = await _userManager.GetRolesAsync(findUser);
            AppUserViewModelinUserService userVm = new AppUserViewModelinUserService();
            userVm.Id = findUser.Id;
            userVm.Avatar = findUser.Avatar;
            userVm.DateCreated = findUser.DateCreated;
            userVm.Email = findUser.Email;
            userVm.FullName = findUser.FullName;
            userVm.UserName = findUser.UserName;
            userVm.Token = findUser.Token;
            userVm.Roles = roles.ToList();
            return userVm;
        }
    }
}