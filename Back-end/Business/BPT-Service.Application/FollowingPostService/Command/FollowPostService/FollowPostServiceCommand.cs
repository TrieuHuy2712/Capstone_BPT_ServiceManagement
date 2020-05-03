using BPT_Service.Application.FollowingPostService.ViewModel;
using BPT_Service.Application.PostService.Query.Extension.GetOwnServiceInformation;
using BPT_Service.Common;
using BPT_Service.Common.Helpers;
using BPT_Service.Common.Logging;
using BPT_Service.Model.Entities;
using BPT_Service.Model.Entities.ServiceModel;
using BPT_Service.Model.Infrastructure.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Newtonsoft.Json;
using System;
using System.Security.Claims;
using System.Threading.Tasks;

namespace BPT_Service.Application.FollowingPostService.Command.FollowPostService
{
    public class FollowPostServiceCommand : IFollowPostServiceCommand
    {
        private readonly IRepository<ServiceFollowing, int> _serviceFollowingRepository;
        private readonly IRepository<Service, Guid> _serviceRepository;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IGetOwnServiceInformationQuery _getOwnServiceInformationQuery;
        private readonly UserManager<AppUser> _userManager;


        public FollowPostServiceCommand(
            IRepository<ServiceFollowing, int> serviceFollowingRepository,
            IRepository<Service, Guid> serviceRepository,
            IHttpContextAccessor httpContextAccessor,
            IGetOwnServiceInformationQuery getOwnServiceInformationQuery,
            UserManager<AppUser> userManager)
        {
            _serviceFollowingRepository = serviceFollowingRepository;
            _serviceRepository = serviceRepository;
            _httpContextAccessor = httpContextAccessor;
            _getOwnServiceInformationQuery = getOwnServiceInformationQuery;
            _userManager = userManager;
        }

        public async Task<CommandResult<ServiceFollowingViewModel>> ExecuteAsync(ServiceFollowingViewModel serviceFollowingViewModel)
        {
            var userId = _httpContextAccessor.HttpContext.User.Identity.Name;
            var userName = _userManager.FindByIdAsync(userId).Result.UserName;
            try
            {
                var checkUserHasFollow = await _serviceFollowingRepository.FindSingleAsync(x =>
                    x.ServiceId == Guid.Parse(serviceFollowingViewModel.ServiceId) &&
                    x.UserId == Guid.Parse(serviceFollowingViewModel.UserId));
                var getOwnerService = await _getOwnServiceInformationQuery.ExecuteAsync(serviceFollowingViewModel.ServiceId);
                if (checkUserHasFollow != null)
                {
                    await Logging<FollowPostServiceCommand>.ErrorAsync(ActionCommand.COMMAND_ADD, userName, "You had follow this service");
                    return new CommandResult<ServiceFollowingViewModel>
                    {
                        isValid = false,
                        errorMessage = "You had follow this service"
                    };
                }
                var postService = await _serviceRepository.FindByIdAsync(Guid.Parse(serviceFollowingViewModel.ServiceId));
                if (postService == null)
                {
                    await Logging<FollowPostServiceCommand>.ErrorAsync(ActionCommand.COMMAND_ADD, userName, ErrorMessageConstant.ERROR_CANNOT_FIND_ID);
                    return new CommandResult<ServiceFollowingViewModel>
                    {
                        isValid = false,
                        errorMessage = ErrorMessageConstant.ERROR_CANNOT_FIND_ID
                    };
                }
                var data = MappingData(serviceFollowingViewModel);
                await _serviceFollowingRepository.Add(data);
                await _serviceFollowingRepository.SaveAsync();
                await LoggingUser<FollowPostServiceCommand>.
                    InformationAsync(getOwnerService, userName, userName + " had follow" + postService.ServiceName);
                await Logging<FollowPostServiceCommand>.InformationAsync(ActionCommand.COMMAND_ADD, userName, JsonConvert.SerializeObject(serviceFollowingViewModel));
                return new CommandResult<ServiceFollowingViewModel>
                {
                    isValid = true,
                    myModel = serviceFollowingViewModel
                };
            }
            catch (Exception ex)
            {
                await Logging<FollowPostServiceCommand>.ErrorAsync(ex, ActionCommand.COMMAND_ADD, userName, "Has error");
                return new CommandResult<ServiceFollowingViewModel>
                {
                    isValid = false,
                    errorMessage = ex.InnerException.Message.ToString()
                };
            }
        }

        private ServiceFollowing MappingData(ServiceFollowingViewModel vm)
        {
            ServiceFollowing serviceFollowing = new ServiceFollowing();
            serviceFollowing.UserId = Guid.Parse(vm.UserId);
            serviceFollowing.ServiceId = Guid.Parse(vm.ServiceId);
            return serviceFollowing;
        }
    }
}