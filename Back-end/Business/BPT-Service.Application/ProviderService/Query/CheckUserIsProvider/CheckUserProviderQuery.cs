using BPT_Service.Application.ProviderService.ViewModel;
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
                var getProvider = await _providerRepository.FindSingleAsync(x => x.UserId == Guid.Parse(userId));
                if (getProvider == null)
                {
                    return new CommandResult<ProviderServiceViewModel>
                    {
                        isValid = false,
                        errorMessage = "Cannot find your provider"
                    };
                }
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
                            isValid = true,
                            myModel = new ProviderServiceViewModel()
                            {
                                Id = getProvider.Id.ToString()
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
                       ErrorAsync(ex.Message.ToString());
                return new CommandResult<ProviderServiceViewModel>
                {
                    isValid = false,
                    errorMessage = ex.InnerException.ToString()
                };
            }
        }
    }
}