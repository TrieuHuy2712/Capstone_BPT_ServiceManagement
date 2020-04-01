using BPT_Service.Application.AuthenticateService.ViewModel;
using BPT_Service.Model.Entities;
using Microsoft.AspNetCore.Identity;
using System.Threading.Tasks;

namespace BPT_Service.Application.AuthenticateService.Query.GetByIdAuthenticateService
{
    public class GetByIdAuthenticateServiceQuery : IGetByIdAuthenticateService
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;

        public GetByIdAuthenticateServiceQuery(UserManager<AppUser> userManager, SignInManager<AppUser> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }

        public async Task<AppUserViewModel> ExecuteAsync(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
            {
                return null;
            }
            else
            {
                AppUserViewModel modelVm = new AppUserViewModel();
                modelVm.Id = user.Id;
                modelVm.UserName = user.UserName;
                modelVm.Avatar = user.Avatar;
                return modelVm;
            }
        }
    }
}