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

namespace BPT_Service.Application.RatingService.Command.DeleteRatingService
{
    public class DeleteRatingServiceCommand : IDeleteRatingServiceCommand
    {
        private readonly IRepository<ServiceRating, int> _serviceRatingRepository;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IRepository<Service, Guid> _serviceRepository;

        public DeleteRatingServiceCommand(
            IRepository<ServiceRating, int> serviceRatingRepository,
            IHttpContextAccessor httpContextAccessor,
            IRepository<Service, Guid> serviceRepository)
        {
            _serviceRatingRepository = serviceRatingRepository;
            _httpContextAccessor = httpContextAccessor;
            _serviceRepository = serviceRepository;
        }

        public async Task<CommandResult<ServiceRating>> ExecuteAsync(int idRating)
        {
            var userName = _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value;
            try
            {
                var getRating = await _serviceRatingRepository.FindByIdAsync(idRating);
                if (getRating == null)
                {
                    return new CommandResult<ServiceRating>
                    {
                        isValid = false,
                        errorMessage = "Cannot find the rating"
                    };
                }
                //Check provider has available
                var getService = await _serviceRepository.FindByIdAsync(getRating.ServiceId);
                if (getService != null)
                {
                    await Logging<DeleteRatingServiceCommand>
                        .WarningAsync(ActionCommand.COMMAND_DELETE, userName, ErrorMessageConstant.ERROR_CANNOT_FIND_ID);
                    return new CommandResult<ServiceRating>
                    {
                        isValid = false,
                        errorMessage = ErrorMessageConstant.ERROR_CANNOT_FIND_ID
                    };
                }
                var getUserId = _httpContextAccessor.HttpContext.User.Identity.Name;
                if (getRating.UserId != Guid.Parse(getUserId))
                {
                    await Logging<DeleteRatingServiceCommand>.
                        WarningAsync(ActionCommand.COMMAND_DELETE, userName, ErrorMessageConstant.ERROR_DELETE_PERMISSION);
                    return new CommandResult<ServiceRating>
                    {
                        isValid = false,
                        errorMessage = ErrorMessageConstant.ERROR_DELETE_PERMISSION
                    };
                }
                _serviceRatingRepository.Remove(getRating);
                await _serviceRatingRepository.SaveAsync();
                await Logging<DeleteRatingServiceCommand>.InformationAsync(ActionCommand.COMMAND_DELETE,
                    userName, JsonConvert.SerializeObject(getUserId));
                return new CommandResult<ServiceRating>
                {
                    isValid = true,
                    myModel = getRating
                };
            }
            catch (Exception ex)
            {
                await Logging<DeleteRatingServiceCommand>.
                    ErrorAsync(ex, ActionCommand.COMMAND_DELETE, userName, "Has error");
                return new CommandResult<ServiceRating>
                {
                    isValid = false,
                    errorMessage = ex.InnerException.Message.ToString()
                };
            }
        }
    }
}