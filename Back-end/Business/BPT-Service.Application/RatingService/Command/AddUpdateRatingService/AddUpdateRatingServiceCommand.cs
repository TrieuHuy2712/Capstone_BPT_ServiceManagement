using BPT_Service.Application.RatingService.ViewModel;
using BPT_Service.Model.Entities;
using BPT_Service.Model.Entities.ServiceModel;
using BPT_Service.Model.Infrastructure.Interfaces;
using Microsoft.AspNetCore.Http;
using System;
using System.Threading.Tasks;

namespace BPT_Service.Application.RatingService.Command.AddRatingService
{
    public class AddUpdateRatingServiceCommand : IAddUpdateRatingServiceCommand
    {
        private readonly IRepository<ServiceRating, int> _serviceRatingRepository;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public AddUpdateRatingServiceCommand(IRepository<ServiceRating, int> serviceRatingRepository, IHttpContextAccessor httpContextAccessor)
        {
            _serviceRatingRepository = serviceRatingRepository;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<CommandResult<ServiceRatingViewModel>> ExecuteAsync(ServiceRatingViewModel serviceRatingViewModel)
        {
            try
            {
                var getUserId = _httpContextAccessor.HttpContext.User.Identity.Name;
                var getServiceRating = await _serviceRatingRepository.FindSingleAsync(x => x.ServiceId == Guid.Parse(serviceRatingViewModel.ServiceId)
                && x.UserId == Guid.Parse(serviceRatingViewModel.UserId));

                if (getServiceRating != null)
                {
                    getServiceRating.NumberOfRating = serviceRatingViewModel.NumberOfRating;
                    getServiceRating.DateModified = DateTime.Now;
                    _serviceRatingRepository.Update(getServiceRating);
                    await _serviceRatingRepository.SaveAsync();
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
                return new CommandResult<ServiceRatingViewModel>
                {
                    isValid = true,
                    myModel = serviceRatingViewModel
                };
            }
            catch (Exception ex)
            {
                return new CommandResult<ServiceRatingViewModel>
                {
                    isValid = false,
                    errorMessage = ex.InnerException.Message.ToString()
                };
            }
        }
    }
}