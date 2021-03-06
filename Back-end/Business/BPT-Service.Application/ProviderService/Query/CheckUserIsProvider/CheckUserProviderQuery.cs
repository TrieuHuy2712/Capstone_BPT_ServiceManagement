using BPT_Service.Application.ProviderService.ViewModel;
using BPT_Service.Common.Constants;
using BPT_Service.Common.Helpers;
using BPT_Service.Common.Logging;
using BPT_Service.Model.Entities;
using BPT_Service.Model.Entities.ServiceModel;
using BPT_Service.Model.Infrastructure.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using System;
using System.Threading.Tasks;

namespace BPT_Service.Application.ProviderService.Query.CheckUserIsProvider
{
    public class CheckUserProviderQuery : ICheckUserIsProviderQuery
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IRepository<Provider, Guid> _providerRepository;

        public CheckUserProviderQuery(
            UserManager<AppUser> userManager,
            IHttpContextAccessor httpContextAccessor,
            IRepository<Provider, Guid> providerRepository
        )
        {
            _userManager = userManager;
            _httpContextAccessor = httpContextAccessor;
            _providerRepository = providerRepository;
        }

        public async Task<CommandResult<ProviderServiceViewModel>> ExecuteAsync(string userId)
        {
            try
            {
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
                    if (item == ConstantRoles.Provider)
                    {
                        return new CommandResult<ProviderServiceViewModel>
                        {
                            isValid = true,
                            myModel = new ProviderServiceViewModel()
                            {
                                Id = getUser.Id.ToString()
                            }
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
                await Logging<CheckUserProviderQuery>.
                       ErrorAsync(ex, ActionCommand.COMMAND_APPROVE, "System", "Has error");
                return new CommandResult<ProviderServiceViewModel>
                {
                    isValid = false,
                    errorMessage = ex.InnerException.ToString()
                };
            }
        }
    }
}