using BPT_Service.Application.FollowingProviderService.ViewModel;
using BPT_Service.Common.Helpers;
using BPT_Service.Common.Logging;
using BPT_Service.Model.Entities;
using BPT_Service.Model.Entities.ServiceModel.ProviderServiceModel;
using BPT_Service.Model.Infrastructure.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Newtonsoft.Json;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace BPT_Service.Application.FollowingProviderService.Command.UnFollowProviderService
{
    public class UnFollowProviderServiceCommand : IUnFollowProviderServiceCommand
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IRepository<ProviderFollowing, int> _providerFollowingRepository;
        private readonly UserManager<AppUser> _userManager;

        public UnFollowProviderServiceCommand(
            IHttpContextAccessor httpContextAccessor,
            IRepository<ProviderFollowing, int> providerFollowingRepository,
            UserManager<AppUser> userManager)
        {
            _httpContextAccessor = httpContextAccessor;
            _providerFollowingRepository = providerFollowingRepository;
            _userManager = userManager;
        }

        public async Task<CommandResult<FollowingProviderServiceViewModel>> ExecuteAsync(FollowingProviderServiceViewModel vm)
        {
            var userId = _httpContextAccessor.HttpContext.User.Identity.Name;
            var userName = await _userManager.FindByIdAsync(userId);
            try
            {
                var checkUserHasBeenFollow = await _providerFollowingRepository.FindAllAsync(x =>
                    x.ProviderId == Guid.Parse(vm.ProviderId) &&
                    x.UserId == Guid.Parse(vm.UserId));

                if (checkUserHasBeenFollow.Count() == 0)
                {
                    await Logging<UnFollowProviderServiceCommand>.ErrorAsync(ActionCommand.COMMAND_DELETE, userName.UserName, "You has not register this service");
                    return new CommandResult<FollowingProviderServiceViewModel>
                    {
                        isValid = false,
                        errorMessage = "You has not register this service"
                    };
                }
                _providerFollowingRepository.RemoveMultiple(checkUserHasBeenFollow.ToList());
                await _providerFollowingRepository.SaveAsync();
                await Logging<UnFollowProviderServiceCommand>.
                    InformationAsync(ActionCommand.COMMAND_DELETE, userName.UserName, "Unfollow Service:" + JsonConvert.SerializeObject(vm));
                return new CommandResult<FollowingProviderServiceViewModel>
                {
                    isValid = true,
                    errorMessage = "You had delete"
                };
            }
            catch (Exception ex)
            {
                await Logging<UnFollowProviderServiceCommand>.ErrorAsync(ex, ActionCommand.COMMAND_DELETE, userName.UserName, "Has error");
                return new CommandResult<FollowingProviderServiceViewModel>
                {
                    isValid = false,
                    errorMessage = ex.InnerException.Message.ToString()
                };
            }
        }
    }
}