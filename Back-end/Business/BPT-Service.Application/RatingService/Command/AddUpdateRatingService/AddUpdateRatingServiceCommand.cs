using BPT_Service.Application.PostService.Query.Extension.GetOwnServiceInformation;
using BPT_Service.Application.RatingService.ViewModel;
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

namespace BPT_Service.Application.RatingService.Command.AddRatingService
{
    public class AddUpdateRatingServiceCommand : IAddUpdateRatingServiceCommand
    {
        private readonly IRepository<ServiceRating, int> _serviceRatingRepository;
        private readonly IGetOwnServiceInformationQuery _getOwnServiceInformationQuery;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IRepository<Service, Guid> _serviceRepository;

        public AddUpdateRatingServiceCommand(
            IRepository<ServiceRating, int> serviceRatingRepository,
            IGetOwnServiceInformationQuery getOwnServiceInformationQuery,
            IHttpContextAccessor httpContextAccessor,
            IRepository<Service, Guid> serviceRepository)
        {
            _serviceRatingRepository = serviceRatingRepository;
            _getOwnServiceInformationQuery = getOwnServiceInformationQuery;
            _httpContextAccessor = httpContextAccessor;
            _serviceRepository = serviceRepository;
        }

        public async Task<CommandResult<ServiceRatingViewModel>> ExecuteAsync(ServiceRatingViewModel serviceRatingViewModel)
        {
            var userName = _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value;
            try
            {
                //Check provider has available
                var getService = await _serviceRepository.FindByIdAsync(Guid.Parse(serviceRatingViewModel.ServiceId));
                if (getService != null)
                {
                    await Logging<AddUpdateRatingServiceCommand>
                        .WarningAsync(ActionCommand.COMMAND_ADD, userName, ErrorMessageConstant.ERROR_CANNOT_FIND_ID);
                    return new CommandResult<ServiceRatingViewModel>
                    {
                        isValid = false,
                        errorMessage = ErrorMessageConstant.ERROR_CANNOT_FIND_ID
                    };
                }
                var getUserId = _httpContextAccessor.HttpContext.User.Identity.Name;
                var getServiceRating = await _serviceRatingRepository.FindSingleAsync(x => x.ServiceId == Guid.Parse(serviceRatingViewModel.ServiceId)
                && x.UserId == Guid.Parse(serviceRatingViewModel.UserId));

                if (getServiceRating != null)
                {
                    var getUserService = _getOwnServiceInformationQuery.ExecuteAsync(serviceRatingViewModel.ServiceId);
                    getServiceRating.NumberOfRating = serviceRatingViewModel.NumberOfRating;
                    getServiceRating.DateModified = DateTime.Now;
                    _serviceRatingRepository.Update(getServiceRating);
                    await _serviceRatingRepository.SaveAsync();
                    await LoggingUser<AddUpdateRatingServiceCommand>.
                        InformationAsync(getUserId, userName, userName + "rated"
                        + getService.ServiceName + " with" + getServiceRating.NumberOfRating);
                    await Logging<AddUpdateRatingServiceCommand>.InformationAsync(ActionCommand.COMMAND_ADD, userName,
                        JsonConvert.SerializeObject(getServiceRating));
                    return new CommandResult<ServiceRatingViewModel>
                    {
                        isValid = true,
                        myModel = serviceRatingViewModel
                    };
                }
                var query = new ServiceRating
                {
                    NumberOfRating = serviceRatingViewModel.NumberOfRating,
                    DateCreated = DateTime.Now,
                    ServiceId = Guid.Parse(serviceRatingViewModel.ServiceId),
                    UserId = Guid.Parse(getUserId)
                };
                await _serviceRatingRepository.Add(query);
                await _serviceRatingRepository.SaveAsync();
                await LoggingUser<AddUpdateRatingServiceCommand>.
                        InformationAsync(getUserId, userName, userName + "rated"
                        + getService.ServiceName + " with" + query.NumberOfRating);
                await Logging<AddUpdateRatingServiceCommand>.InformationAsync(ActionCommand.COMMAND_ADD, userName,
                    JsonConvert.SerializeObject(query));
                return new CommandResult<ServiceRatingViewModel>
                {
                    isValid = true,
                    myModel = serviceRatingViewModel
                };
            }
            catch (Exception ex)
            {
                await Logging<AddUpdateRatingServiceCommand>.
                    ErrorAsync(ex, ActionCommand.COMMAND_ADD, userName, "Has Error");
                return new CommandResult<ServiceRatingViewModel>
                {
                    isValid = false,
                    errorMessage = ex.InnerException.Message.ToString()
                };
            }
        }
    }
}