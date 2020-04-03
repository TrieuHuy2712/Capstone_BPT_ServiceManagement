using BPT_Service.Application.FollowingPostService.ViewModel;
using BPT_Service.Common;
using BPT_Service.Common.Helpers;
using BPT_Service.Common.Logging;
using BPT_Service.Model.Entities;
using BPT_Service.Model.Entities.ServiceModel;
using BPT_Service.Model.Infrastructure.Interfaces;
using Microsoft.AspNetCore.Http;
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

        public UnFollowPostServiceCommand(
            IRepository<ServiceFollowing, int> serviceFollowingRepository,
            IHttpContextAccessor httpContextAccessor)
        {
            _serviceFollowingRepository = serviceFollowingRepository;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<CommandResult<ServiceFollowingViewModel>> ExecuteAsync(ServiceFollowingViewModel model)
        {
            var userName = _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value;
            try
            {
                var checkUserHasFollow = await _serviceFollowingRepository.FindByIdAsync(model.Id);

                if (checkUserHasFollow == null)
                {
                    await Logging<UnFollowPostServiceCommand>.
                        ErrorAsync(ActionCommand.COMMAND_ADD, userName, ErrorMessageConstant.ERROR_CANNOT_FIND_ID);
                    return new CommandResult<ServiceFollowingViewModel>
                    {
                        isValid = false,
                        errorMessage = ErrorMessageConstant.ERROR_CANNOT_FIND_ID
                    };
                }
                _serviceFollowingRepository.Remove(checkUserHasFollow);
                await _serviceFollowingRepository.SaveAsync();
                await Logging<UnFollowPostServiceCommand>
                    .InformationAsync(ActionCommand.COMMAND_ADD, userName, JsonConvert.SerializeObject(checkUserHasFollow));
                return new CommandResult<ServiceFollowingViewModel>
                {
                    isValid = true,
                    myModel = model
                };
            }
            catch (Exception ex)
            {
                await Logging<UnFollowPostServiceCommand>.ErrorAsync(ex, ActionCommand.COMMAND_UPDATE, userName, "Has error");
                return new CommandResult<ServiceFollowingViewModel>
                {
                    isValid = false,
                    errorMessage = ex.InnerException.Message.ToString()
                };
            }
        }
    }
}