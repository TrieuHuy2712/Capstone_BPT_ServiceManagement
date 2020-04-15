using BPT_Service.Model.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using System;
using System.Threading.Tasks;

namespace BPT_Service.Application.AuthenticateService.Query.CheckCanAccessMain
{
    public class CheckCanAccessMain : ICheckCanAccessMain
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly RoleManager<AppRole> _roleManager;

        public CheckCanAccessMain(
            UserManager<AppUser> userManager,
            IHttpContextAccessor httpContextAccessor,
            RoleManager<AppRole> roleManager)
        {
            _userManager = userManager;
            _httpContextAccessor = httpContextAccessor;
            _roleManager = roleManager;
        }

        public async Task<bool> ExecuteAsync(string userName)
        {
            try
            {
                var userId = await _userManager.FindByNameAsync(userName);
                //Get Role Of User
                //var getUser = await _userManager.FindByIdAsync(userId);
                var findUser = await _userManager.GetRolesAsync(userId);
                //Remove
                //Check don't have Provider Or Customer
                var count = 0;
                foreach (var item in findUser)
                {
                    if (item != "Provider" || item != "Customer")
                    {
                        count++;
                    }
                }
                if (count == 0)
                {
                    return false;
                }
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
    }
}