using BPT_Service.Application.FollowingPostService.ViewModel;
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

namespace BPT_Service.Application.FollowingPostService.Command.UnFollowPostService
{
    public class UnFollowPostServiceCommand : IUnFollowPostServiceCommand
    {
        private readonly IRepository<ServiceFollowing, int> _serviceFollowingRepository;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly UserManager<AppUser> _userManager;

        public UnFollowPostServiceCommand(
            IRepository<ServiceFollowing, int> serviceFollowingRepository,
            IHttpContextAccessor httpContextAccessor,
            UserManager<AppUser> userManager)
        {
            _serviceFollowingRepository = serviceFollowingRepository;
            _httpContextAccessor = httpContextAccessor;
            _userManager = userManager;
        }

        public async Task<CommandResult<ServiceFollowingViewModel>> ExecuteAsync(ServiceFollowingViewModel model)
        {
            var userId = _httpContextAccessor.HttpContext.User.Identity.Name;
            var userName = await _userManager.FindByIdAsync(userId);
            try
            {
                var checkUserHasFollow = await _serviceFollowingRepository.FindByIdAsync(model.Id);

                if (checkUserHasFollow == null)
                {
                    await Logging<UnFollowPostServiceCommand>.
                        ErrorAsync(ActionCommand.COMMAND_ADD, userName.UserName, ErrorMessageConstant.ERROR_CANNOT_FIND_ID);
                    return new CommandResult<ServiceFollowingViewModel>
                    {
                        isValid = false,
                        errorMessage = ErrorMessageConstant.ERROR_CANNOT_FIND_ID
                    };
                }
                _serviceFollowingRepository.Remove(checkUserHasFollow);
                await _serviceFollowingRepository.SaveAsync();
                await Logging<UnFollowPostServiceCommand>
                    .InformationAsync(ActionCommand.COMMAND_ADD, userName.UserName, JsonConvert.SerializeObject(checkUserHasFollow));
                return new CommandResult<ServiceFollowingViewModel>
                {
                    isValid = true,
                    myModel = model
                };
            }
            catch (Exception ex)
            {
                await Logging<UnFollowPostServiceCommand>.ErrorAsync(ex, ActionCommand.COMMAND_UPDATE, userName.UserName, "Has error");
                return new CommandResult<ServiceFollowingViewModel>
                {
                    isValid = false,
                    errorMessage = ex.InnerException.Message.ToString()
                };
            }
        }
    }
}