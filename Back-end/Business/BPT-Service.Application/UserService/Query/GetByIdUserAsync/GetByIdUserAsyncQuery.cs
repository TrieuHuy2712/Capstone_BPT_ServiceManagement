using System.Linq;
using System.Threading.Tasks;
using BPT_Service.Application.UserService.ViewModel;
using BPT_Service.Model.Entities;
using Microsoft.AspNetCore.Identity;

namespace BPT_Service.Application.UserService.Query.GetByIdAsync
{


    public class GetByIdUserAsyncQuery : IGetByIdUserAsyncQuery
    {
        private readonly UserManager<AppUser> _userManager;
        public GetByIdUserAsyncQuery(UserManager<AppUser> userManager)
        {
            _userManager = userManager;
        }

        public async Task<AppUserViewModelinUserService> ExcecuteAsync(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            var roles = await _userManager.GetRolesAsync(user);
            AppUserViewModelinUserService userVm = new AppUserViewModelinUserService();
            userVm.Id = user.Id;
            userVm.Avatar = user.Avatar;
            userVm.DateCreated = user.DateCreated;
            userVm.Email = user.Email;
            userVm.FullName = user.FullName;
            userVm.UserName = user.UserName;
            userVm.Token = user.Token;
            userVm.Roles = roles.ToList();
            return userVm;
        }
    }
}