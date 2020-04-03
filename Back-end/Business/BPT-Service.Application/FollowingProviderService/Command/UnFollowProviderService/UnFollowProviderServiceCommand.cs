using BPT_Service.Application.FollowingProviderService.ViewModel;
using BPT_Service.Common.Helpers;
using BPT_Service.Common.Logging;
using BPT_Service.Model.Entities;
using BPT_Service.Model.Entities.ServiceModel.ProviderServiceModel;
using BPT_Service.Model.Infrastructure.Interfaces;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace BPT_Service.Application.FollowingProviderService.Command.UnFollowProviderService
{
    public class UnFollowProviderServiceCommand : IUnFollowProviderServiceCommand
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IRepository<ProviderFollowing, int> _providerFollowingRepository;

        public UnFollowProviderServiceCommand(
            IHttpContextAccessor httpContextAccessor,
            IRepository<ProviderFollowing, int> providerFollowingRepository)
        {
            _httpContextAccessor = httpContextAccessor;
            _providerFollowingRepository = providerFollowingRepository;
        }

        public async Task<CommandResult<FollowingProviderServiceViewModel>> ExecuteAsync(FollowingProviderServiceViewModel vm)
        {
            var userName = _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value;
            try
            {
                var checkUserHasBeenFollow = await _providerFollowingRepository.FindAllAsync(x =>
                    x.ProviderId == Guid.Parse(vm.ProviderId) &&
                    x.UserId == Guid.Parse(vm.UserId));

                if (checkUserHasBeenFollow.Count() == 0)
                {
                    await Logging<UnFollowProviderServiceCommand>.ErrorAsync(ActionCommand.COMMAND_DELETE, userName, "You has not register this service");
                    return new CommandResult<FollowingProviderServiceViewModel>
                    {
                        isValid = false,
                        errorMessage = "You has not register this service"
                    };
                }
                _providerFollowingRepository.RemoveMultiple(checkUserHasBeenFollow.ToList());
                await _providerFollowingRepository.SaveAsync();
                await Logging<UnFollowProviderServiceCommand>.
                    InformationAsync(ActionCommand.COMMAND_DELETE, userName, "Unfollow Service:" + JsonConvert.SerializeObject(vm));
                return new CommandResult<FollowingProviderServiceViewModel>
                {
                    isValid = true,
                    errorMessage = "You had delete"
                };
            }
            catch (Exception ex)
            {
                await Logging<UnFollowProviderServiceCommand>.ErrorAsync(ex, ActionCommand.COMMAND_DELETE, userName, "Has error");
                return new CommandResult<FollowingProviderServiceViewModel>
                {
                    isValid = false,
                    errorMessage = ex.InnerException.Message.ToString()
                };
            }
        }
    }
}