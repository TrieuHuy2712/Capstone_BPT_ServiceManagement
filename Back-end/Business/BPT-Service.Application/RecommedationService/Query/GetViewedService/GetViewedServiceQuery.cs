using BPT_Service.Application.RecommedationService.ViewModel;
using BPT_Service.Model;
using BPT_Service.Model.Entities;
using BPT_Service.Model.Entities.ServiceModel;
using BPT_Service.Model.Infrastructure.Interfaces;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BPT_Service.Application.RecommedationService.Query.GetViewedService
{
    public class GetViewedServiceQuery : IGetViewedServiceQuery
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IRepository<Category, int> _categoryRepository;
        private readonly IRepository<Model.Entities.ServiceModel.TagService, int> _tagServiceRepository;
        private readonly IRepository<Service, Guid> _serviceRepository;
        private readonly IRepository<ServiceImage, int> _imageRepository;
        private readonly IRepository<ServiceRating, int> _ratingRepository;
        private readonly IRepository<Tag, Guid> _tagRepository;
        private readonly IRepository<UserRecommendation, int> _userRecommendationRepository;

        public GetViewedServiceQuery(
            IHttpContextAccessor httpContextAccessor, 
            IRepository<Category, int> categoryRepository, 
            IRepository<Model.Entities.ServiceModel.TagService, int> tagServiceRepository, 
            IRepository<Service, Guid> serviceRepository, 
            IRepository<ServiceImage, int> imageRepository, 
            IRepository<ServiceRating, int> ratingRepository, 
            IRepository<Tag, Guid> tagRepository, 
            IRepository<UserRecommendation, int> userRecommendationRepository)
        {
            _httpContextAccessor = httpContextAccessor;
            _categoryRepository = categoryRepository;
            _tagServiceRepository = tagServiceRepository;
            _serviceRepository = serviceRepository;
            _imageRepository = imageRepository;
            _ratingRepository = ratingRepository;
            _tagRepository = tagRepository;
            _userRecommendationRepository = userRecommendationRepository;
        }

        public async Task<List<ServiceRecommendationViewModel>> ExecuteAsync()
        {
            try
            {
                var getUserId = _httpContextAccessor.HttpContext.User.Identity.Name;
                var groupAllInformation = await _userRecommendationRepository.
                    FindAllAsync(x => x.UserId == Guid.Parse(getUserId));
                var getAllService = await _serviceRepository.FindAllAsync(x => x.Status == Model.Enums.Status.Active);

                //Var get all rating service
                var allRating = await _ratingRepository.FindAllAsync();
                // Return list content Rating
                var returListRecommendation = (from serviceRecommend in groupAllInformation
                                               join service in getAllService
                                               on serviceRecommend.ServiceId equals service.Id
                                               join rating in allRating.DefaultIfEmpty()
                                               on service.Id equals rating.ServiceId into ps
                                               select new ServiceRecommendationViewModel()
                                               {
                                                   IdService = service.Id.ToString(),
                                                   ImgService = _imageRepository.FindSingleAsync(x => x.ServiceId == service.Id && x.isAvatar).Result.Path,
                                                   NameService = service.ServiceName,
                                                   Rating = ps == null ? 0 : ps.Select(x => x.NumberOfRating).Average(),
                                               }).ToList();
                return returListRecommendation;
            }
            catch (Exception)
            {

                throw;
            }
        }
    }
}
