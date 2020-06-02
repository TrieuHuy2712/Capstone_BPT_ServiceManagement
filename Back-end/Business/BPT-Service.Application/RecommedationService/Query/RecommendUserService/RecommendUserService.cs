using BPT_Service.Application.RecommedationService.ViewModel;
using BPT_Service.Model;
using BPT_Service.Model.Entities;
using BPT_Service.Model.Entities.ServiceModel;
using BPT_Service.Model.Infrastructure.Interfaces;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BPT_Service.Application.RecommedationService.Query.RecommendUserService
{
    public class RecommendUserService : IRecommendUserService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IRepository<Category, int> _categoryRepository;
        private readonly IRepository<Model.Entities.ServiceModel.TagService, int> _tagServiceRepository;
        private readonly IRepository<Service, Guid> _serviceRepository;
        private readonly IRepository<ServiceImage, int> _imageRepository;
        private readonly IRepository<ServiceRating, int> _ratingRepository;
        private readonly IRepository<Tag, Guid> _tagRepository;

        private readonly IRepository<UserRecommendation, int> _userRecommendationRepository;

        public RecommendUserService(
            IHttpContextAccessor httpContextAccessor, 
            IRepository<Category, int> categoryRepository, 
            IRepository<Model.Entities.ServiceModel.TagService, int> tagServiceRepository, 
            IRepository<Service, Guid> serviceRepository, 
            IRepository<ServiceImage, int> imageRepository, 
            IRepository<ServiceRating, int> ratingRepository, 
            IRepository<Tag, Guid> tagRepository)
        {
            _httpContextAccessor = httpContextAccessor;
            _categoryRepository = categoryRepository;
            _tagServiceRepository = tagServiceRepository;
            _serviceRepository = serviceRepository;
            _imageRepository = imageRepository;
            _ratingRepository = ratingRepository;
            _tagRepository = tagRepository;
        }

        public async Task<List<ServiceRecommendationViewModel>> ExecuteAsync()
        {
            try
            {
                var getUserId = _httpContextAccessor.HttpContext.User.Identity.Name;
                var groupAllInformation = await _userRecommendationRepository.
                    FindAllAsync(x => x.UserId == Guid.Parse(getUserId));
                var getAllService = await _serviceRepository.FindAllAsync(x => x.Status == Model.Enums.Status.Active);
                var getRecommendByCategory = await RecommendationByCategory(groupAllInformation, getAllService);
                var getRecommendByTag = await RecommendationByTag(groupAllInformation, getAllService);
                getRecommendByCategory.AddRange(getRecommendByTag);

                // Get List Service has viewed for except
                var listServiceHasViewed = groupAllInformation.Select(x => new ServiceRecommendationViewModel()
                {
                    IdService = x.ServiceId.ToString()
                }).ToList();
                var listServiceRecommend = getRecommendByCategory.Distinct().ToList();
                listServiceRecommend.Except(listServiceHasViewed);

                //Var get all rating service
                var allRating = await _ratingRepository.FindAllAsync();
                // Return list content Rating
                var returListRecommendation = (from serviceRecommend in listServiceRecommend.ToList()
                                               join service in getAllService.ToList()
                                               on Guid.Parse(serviceRecommend.IdService) equals service.Id
                                               join rating in allRating.ToList().DefaultIfEmpty()
                                               on service.Id equals rating.ServiceId into ps
                                               select new ServiceRecommendationViewModel()
                                               {
                                                   IdService = service.Id.ToString(),
                                                   ImgService = _imageRepository.FindSingleAsync(x => x.ServiceId == service.Id && x.isAvatar).Result.Path,
                                                   NameService = service.ServiceName,
                                                   Rating = ps == null ? 0 : ps.Select(x=>x.NumberOfRating).ToList().Average(),
                                               }).ToList();
                return returListRecommendation;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<List<ServiceRecommendationViewModel>> RecommendationByCategory(IEnumerable<UserRecommendation> recommendations, IEnumerable<Service> listService)
        {
            //var getListService = await _serviceRepository.FindAllAsync(x=>x.Status== Model.Enums.Status.Active);

            // Get Recommend
            var getListCategoryRecommend = (from recommend in recommendations.ToList()
                                            join service in listService.ToList()
                                            on recommend.ServiceId equals service.Id
                                            join category in await _categoryRepository.FindAllAsync()
                                            on service.CategoryId equals category.Id
                                            select new
                                            {
                                                categoryId = service.Id
                                            }).ToList();

            // Get List Service By Recommend
            var getRecommendService = (from service in listService.ToList()
                                       join categoryRecommend in getListCategoryRecommend
                                       on service.Id equals categoryRecommend.categoryId
                                       select new ServiceRecommendationViewModel
                                       {
                                           IdService = service.Id.ToString(),
                                           NameService = service.ServiceName,
                                       }).ToList();
            return getRecommendService;
        }

        // Get List Service By Tag
        public async Task<List<ServiceRecommendationViewModel>> RecommendationByTag(IEnumerable<UserRecommendation> recommendations, IEnumerable<Service> listService)
        {
            //var getListService = await _serviceRepository.FindAllAsync(x => x.Status == Model.Enums.Status.Active);

            var getServiceTag = await _tagServiceRepository.FindAllAsync();
            var getTag = await _tagRepository.FindAllAsync();
            // Get Recommend
            var getListCategoryRecommend = (from recommend in recommendations.ToList()
                                            join service in listService.ToList()
                                            on recommend.ServiceId equals service.Id
                                            join servTag in getServiceTag.ToList()
                                            on service.Id equals servTag.ServiceId
                                            join tag in getTag.ToList()
                                            on servTag.TagId equals tag.Id
                                            select new
                                            {
                                                TagId = tag.Id
                                            }).ToList();

            // Get Recommend
            var getListRecommend = (from service in listService.ToList()
                                    join servTag in getServiceTag.ToList()
                                    on service.Id equals servTag.ServiceId
                                    join tag in getTag.ToList()
                                    on servTag.TagId equals tag.Id
                                    select new ServiceRecommendationViewModel
                                    {
                                        IdService = service.Id.ToString()
                                    }).ToList();

            return getListRecommend;
        }
    }
}