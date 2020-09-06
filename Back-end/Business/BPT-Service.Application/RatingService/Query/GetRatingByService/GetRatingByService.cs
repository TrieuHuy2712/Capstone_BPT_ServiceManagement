using BPT_Service.Application.RatingService.ViewModel;
using BPT_Service.Model.Entities;
using BPT_Service.Model.Entities.ServiceModel;
using BPT_Service.Model.Infrastructure.Interfaces;
using Microsoft.AspNetCore.Identity;
using System;
using System.Threading.Tasks;
using System.Linq;

namespace BPT_Service.Application.RatingService.Query.GetRatingByService
{
    public class GetRatingByService : IGetRatingByService
    {
        private IRepository<ServiceRating, int> _ratingServiceRepository;
        private UserManager<AppUser> _userManager;

        public GetRatingByService(IRepository<ServiceRating, int> ratingServiceRepository, UserManager<AppUser> userManager)
        {
            _ratingServiceRepository = ratingServiceRepository;
            _userManager = userManager;
        }

        public async Task<ListRatingByServiceViewModel> ExecuteAsync(string idService)
        {
            var findAllService = await _ratingServiceRepository.FindAllAsync(x => x.ServiceId == Guid.Parse(idService));
            var result = new ListRatingByServiceViewModel
            {
                AverageRating = findAllService.Average(x => x.NumberOfRating),
                NumberRating = findAllService.Count(),
                listRating = findAllService.Select(x => new ServiceRatingViewModel
                {
                    UserNameWithEmail = _userManager.FindByIdAsync(x.UserId.ToString()).Result.Email,
                    NumberOfRating = x.NumberOfRating,
                }).ToList()
            };
            return result;
        }
    }
}