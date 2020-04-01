using BPT_Service.Application.AuthenticateService.ViewModel;
using BPT_Service.Model.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BPT_Service.Application.AuthenticateService.Query.GetAllAuthenticateService
{
    public class GetAllAuthenticateServiceQuery : IGetAllAuthenticateServiceQuery
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;

        public GetAllAuthenticateServiceQuery(UserManager<AppUser> userManager, SignInManager<AppUser> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }

        public async Task<IEnumerable<AppUserViewModel>> ExecuteAsync()
        {
            {
                var model = await _userManager.Users.ToListAsync();
                IEnumerable<AppUserViewModel> modelVm = model.Select(x => new AppUserViewModel
                {
                    Id = x.Id,
                    UserName = x.UserName,
                    Avatar = x.Avatar,
                });
                return modelVm;
            }
        }
    }
}