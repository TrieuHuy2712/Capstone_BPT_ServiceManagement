using System.Threading.Tasks;
using BPT_Service.Application.ProviderService.ViewModel;
using BPT_Service.Model.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;

namespace BPT_Service.Application.ProviderService.Query.CheckUserIsProvider
{
    public class CheckUserProviderQuery : ICheckUserIsProviderQuery
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public CheckUserProviderQuery(
            UserManager<AppUser> userManager,
            IHttpContextAccessor httpContextAccessor
        )
        {
            _userManager = userManager;
            _httpContextAccessor = httpContextAccessor;
        }
        public async Task<CommandResult<ProviderServiceViewModel>> ExecuteAsync()
        {
            try
            {
                var userId = _httpContextAccessor.HttpContext.User.Identity.Name;
                var getUser = await _userManager.FindByIdAsync(userId);
                if (getUser == null)
                {
                    return new CommandResult<ProviderServiceViewModel>
                    {
                        isValid = false,
                        errorMessage = "Cannot find your user"
                    };
                }
                var getRoleOfUser = await _userManager.GetRolesAsync(getUser);
                foreach (var item in getRoleOfUser)
                {
                    if (item == "Provider")
                    {
                        return new CommandResult<ProviderServiceViewModel>
                        {
                            isValid = true
                        };
                    }
                }
                return new CommandResult<ProviderServiceViewModel>
                {
                    isValid = false,
                    errorMessage = "User is not in role Provider"
                };
            }
            catch (System.Exception ex)
            {

                return new CommandResult<ProviderServiceViewModel>
                {
                    isValid = false,
                    errorMessage = ex.InnerException.ToString()
                };
            }
        }
    }
}