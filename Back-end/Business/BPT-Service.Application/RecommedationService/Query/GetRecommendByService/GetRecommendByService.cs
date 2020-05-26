using BPT_Service.Application.RecommedationService.ViewModel;
using BPT_Service.Model.Entities;
using BPT_Service.Model.Entities.ServiceModel;
using BPT_Service.Model.Enums;
using BPT_Service.Model.Infrastructure.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BPT_Service.Application.RecommedationService.Query.GetRecommendByService
{
    public class GetRecommendByService : IGetRecommendByService
    {
        private readonly IRepository<Recommendation, int> _recommendRepository;
        private readonly IRepository<Service, Guid> _serviceRepository;
        private readonly IRepository<ServiceRating, int> _ratingRepository;
        private readonly IRepository<ServiceImage, int> _serviceImageRepository;

        public GetRecommendByService(
            IRepository<Recommendation, int> recommendRepository,
            IRepository<Service, Guid> serviceRepository,
            IRepository<ServiceRating, int> ratingRepository,
            IRepository<ServiceImage, int> serviceImageRepository)
        {
            _recommendRepository = recommendRepository;
            _serviceRepository = serviceRepository;
            _ratingRepository = ratingRepository;
            _serviceImageRepository = serviceImageRepository;
        }

        public async Task<List<ServiceRecommendationViewModel>> ExecuteAsync(bool isSetDefault)
        {
            try
            {
                var getRecommendService = await _recommendRepository.FindAllAsync(x => x.Type == TypeRecommendation.Service);
                List<ServiceRecommendationViewModel> listRecommend = new List<ServiceRecommendationViewModel>();
                if (isSetDefault)
                {
                    _recommendRepository.RemoveMultiple(getRecommendService.ToList());

                    var getAllRatingService = await _ratingRepository.FindAllAsync();
                    var groupCountService = getAllRatingService.GroupBy(x => x.ServiceId).Select(t => new
                    {
                        IdService = t.Key,
                        Average = t.Average(x => x.NumberOfRating)
                    }).ToList();

                    var joinService = (from service in await _serviceRepository.FindAllAsync()
                                       join rating in groupCountService.DefaultIfEmpty()
                                       on service.Id equals rating.IdService into ps
                                       select new
                                       {
                                           IdService = service.Id,
                                           Rating = ps == null ? 0 : ps.Select(x => x.Average).FirstOrDefault()
                                       }).OrderByDescending(x => x.Rating).Take(10).ToList();

                    //Add new Information
                    var countIncrement = 0;
                    foreach (var item in joinService)
                    {
                        var findInformation = await _serviceRepository.FindByIdAsync(item.IdService);
                        var addRecommend = new Recommendation()
                        {
                            IdType = findInformation.Id.ToString(),
                            Order = ++countIncrement,
                            Type = TypeRecommendation.Service
                        };
                        await _recommendRepository.Add(addRecommend);
                        var findImage = await _serviceImageRepository.
                                FindSingleAsync(x => x.ServiceId == findInformation.Id && x.isAvatar);
                        var recommend = new ServiceRecommendationViewModel()
                        {
                            Id = addRecommend.Id,
                            IdService = findInformation.Id.ToString(),
                            ImgService = !string.IsNullOrEmpty(findImage.Path) ? findImage.Path : "",
                            NameService = findInformation.ServiceName,
                            Rating = item.Rating,
                            Order = countIncrement,
                        };
                        listRecommend.Add(recommend);
                    }
                    await _recommendRepository.SaveAsync();
                }
                else
                {
                    foreach (var item in getRecommendService.OrderBy(x => x.Order).ToList())
                    {
                        Guid numberParse;
                        if (Guid.TryParse(item.IdType, out numberParse))
                        {
                            var findInformation = await _serviceRepository.FindByIdAsync(numberParse);
                            var findImage = await _serviceImageRepository.
                                FindSingleAsync(x => x.ServiceId == findInformation.Id && x.isAvatar);
                            var findRating = await _ratingRepository.FindAllAsync(x => x.ServiceId == findInformation.Id);
                            listRecommend.Add(new ServiceRecommendationViewModel()
                            {
                                IdService = findInformation.Id.ToString(),
                                ImgService = !string.IsNullOrEmpty(findImage.Path) ? findImage.Path : "",
                                NameService = findInformation.ServiceName,
                                Rating = findRating.Count() > 0 ? findRating.GroupBy(x => x.ServiceId).Select(t => new
                                {
                                    Average = t.Average(x => x.NumberOfRating)
                                }).FirstOrDefault().Average : 0,
                                Id = item.Id,
                                Order = item.Order
                            });
                        }
                    }
                }
                return listRecommend.ToList();
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}