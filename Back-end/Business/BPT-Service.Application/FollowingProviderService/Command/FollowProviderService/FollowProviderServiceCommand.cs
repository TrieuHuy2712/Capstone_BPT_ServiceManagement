using BPT_Service.Application.FollowingProviderService.ViewModel;
using BPT_Service.Common;
using BPT_Service.Common.Helpers;
using BPT_Service.Common.Logging;
using BPT_Service.Model.Entities;
using BPT_Service.Model.Entities.ServiceModel;
using BPT_Service.Model.Entities.ServiceModel.ProviderServiceModel;
using BPT_Service.Model.Infrastructure.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Newtonsoft.Json;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace BPT_Service.Application.FollowingProviderService.Command.FollowProviderService
{
    public class FollowProviderServiceCommand : IFollowProviderServiceCommand
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IRepository<Provider, Guid> _providerRepository;
        private readonly IRepository<ProviderFollowing, int> _providerFollowingRepository;
        private readonly UserManager<AppUser> _userManager;

        public FollowProviderServiceCommand(
            IHttpContextAccessor httpContextAccessor,
            IRepository<Provider, Guid> providerRepository,
            IRepository<ProviderFollowing, int> providerFollowingRepository,
            UserManager<AppUser> userManager)
        {
            _httpContextAccessor = httpContextAccessor;
            _providerRepository = providerRepository;
            _providerFollowingRepository = providerFollowingRepository;
            _userManager = userManager;
        }

        public async Task<CommandResult<FollowingProviderServiceViewModel>> ExecuteAsync(FollowingProviderServiceViewModel vm)
        {
            var userId = _httpContextAccessor.HttpContext.User.Identity.Name;
            var userName = await _userManager.FindByIdAsync(userId);
            try
            {
                var checkUserHasFollowProvider = await _providerFollowingRepository.FindAllAsync(x =>
                    x.ProviderId == Guid.Parse(vm.ProviderId) && x.UserId == Guid.Parse(vm.UserId));
                if (checkUserHasFollowProvider.Count() != 0)
                {
                    await Logging<FollowProviderServiceCommand>.
                        ErrorAsync(ActionCommand.COMMAND_ADD, userName.UserName, userName.UserName + "had follow provider");
                    return new CommandResult<FollowingProviderServiceViewModel>
                    {
                        isValid = false,
                        errorMessage = "You had follow this provider"
                    };
                }
                var getProvider = await _providerRepository.FindByIdAsync(Guid.Parse(vm.ProviderId));
                if (getProvider == null)
                {
                    await Logging<FollowProviderServiceCommand>.
                       ErrorAsync(ActionCommand.COMMAND_ADD, userName.UserName, ErrorMessageConstant.ERROR_CANNOT_FIND_ID);
                    return new CommandResult<FollowingProviderServiceViewModel>
                    {
                        isValid = false,
                        errorMessage = ErrorMessageConstant.ERROR_CANNOT_FIND_ID
                    };
                }
                var data = MappingData(vm);
                await _providerFollowingRepository.Add(data);
                await _providerFollowingRepository.SaveAsync();
                await LoggingUser<FollowProviderServiceCommand>.
                    InformationAsync(getProvider.UserId.ToString(), userName.UserName, userName.UserName + " had follow your provider");
                await Logging<FollowProviderServiceCommand>.InformationAsync(ActionCommand.COMMAND_ADD, userName.UserName, JsonConvert.SerializeObject(data));
                return new CommandResult<FollowingProviderServiceViewModel>
                {
                    isValid = true,
                    myModel = vm
                };
            }
            catch (Exception ex)
            {
                await Logging<FollowProviderServiceCommand>.ErrorAsync(ex, ActionCommand.COMMAND_ADD, userName.UserName, "Has error:");
                return new CommandResult<FollowingProviderServiceViewModel>
                {
                    isValid = false,
                    errorMessage = ex.InnerException.Message.ToString()
                };
            }
        }

        private ProviderFollowing MappingData(FollowingProviderServiceViewModel vm)
        {
            ProviderFollowing providerFollowing = new ProviderFollowing();
            providerFollowing.UserId = Guid.Parse(vm.UserId);
            providerFollowing.ProviderId = Guid.Parse(vm.ProviderId);
            providerFollowing.IsReceiveEmail = vm.IsReceiveEmail;
            providerFollowing.DateCreated = DateTime.Now;
            return providerFollowing;
        }
    }
}