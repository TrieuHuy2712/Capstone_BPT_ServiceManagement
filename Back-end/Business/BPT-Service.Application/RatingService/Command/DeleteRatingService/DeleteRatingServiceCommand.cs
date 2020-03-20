using BPT_Service.Model.Entities;
using BPT_Service.Model.Entities.ServiceModel;
using BPT_Service.Model.Infrastructure.Interfaces;
using Microsoft.AspNetCore.Http;
using System;
using System.Threading.Tasks;

namespace BPT_Service.Application.RatingService.Command.DeleteRatingService
{
    public class DeleteRatingServiceCommand : IDeleteRatingServiceCommand
    {
        private readonly IRepository<ServiceRating, int> _serviceRatingRepository;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public DeleteRatingServiceCommand(IRepository<ServiceRating, int> serviceRatingRepository, IHttpContextAccessor httpContextAccessor)
        {
            _serviceRatingRepository = serviceRatingRepository;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<CommandResult<ServiceRating>> ExecuteAsync(int idRating)
        {
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
                var getUserId = _httpContextAccessor.HttpContext.User.Identity.Name;
                if (getRating.UserId != Guid.Parse(getUserId))
                {
                    return new CommandResult<ServiceRating>
                    {
                        isValid = false,
                        errorMessage = "You don't have permission"
                    };
                }
                _serviceRatingRepository.Remove(getRating);
                await _serviceRatingRepository.SaveAsync();
                return new CommandResult<ServiceRating>
                {
                    isValid = true,
                    myModel = getRating
                };
            }
            catch (Exception ex)
            {
                return new CommandResult<ServiceRating>
                {
                    isValid = false,
                    errorMessage = ex.InnerException.Message.ToString()
                };
            }
        }
    }
}