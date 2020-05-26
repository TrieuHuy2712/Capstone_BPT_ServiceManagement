using BPT_Service.Application.RecommedationService.ViewModel;
using BPT_Service.Model.Entities;
using BPT_Service.Model.Entities.ServiceModel;
using BPT_Service.Model.Enums;
using BPT_Service.Model.Infrastructure.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BPT_Service.Application.RecommedationService.Query.GetRecommendByLocation
{
    public class GetRecommendByLocation : IGetRecommendByLocation
    {
        private readonly IRepository<Recommendation, int> _recommendRepository;
        private readonly IRepository<CityProvince, int> _locationRepository;
        private readonly IRepository<Provider, Guid> _providerRepository;

        public GetRecommendByLocation(IRepository<Recommendation, int> recommendRepository, IRepository<CityProvince, int> locationRepository, IRepository<Provider, Guid> providerRepository)
        {
            _recommendRepository = recommendRepository;
            _locationRepository = locationRepository;
            _providerRepository = providerRepository;
        }

        public async Task<List<LocationRecommendationViewModel>> ExecuteAsync(bool isSetDefault)
        {
            try
            {
                var getRecommendLocation = await _recommendRepository.FindAllAsync(x => x.Type == TypeRecommendation.Location);
                List<LocationRecommendationViewModel> listRecommend = new List<LocationRecommendationViewModel>();
                if (isSetDefault)
                {
                    _recommendRepository.RemoveMultiple(getRecommendLocation.ToList());

                    //Get all
                    var q = (from location in await _locationRepository.FindAllAsync()
                             join provider in await _providerRepository.FindAllAsync()
                             on location.Id equals provider.CityId into ps
                             select new
                             {
                                 NameLocation = location.Id,
                                 NumberOfCity = ps.Count()
                             }).OrderByDescending(x => x.NumberOfCity).Take(10);

                    //Add new Information
                    var countIncrement = 0;
                    foreach (var item in q)
                    {
                        var findInformation = await _locationRepository.FindByIdAsync(item.NameLocation);
                        var addRecommend = new Recommendation()
                        {
                            IdType = findInformation.Id.ToString(),
                            Order = ++countIncrement,
                            Type = TypeRecommendation.Location
                        };
                        await _recommendRepository.Add(addRecommend);
                        var recommend = new LocationRecommendationViewModel()
                        {
                            Id = addRecommend.Id,
                            IdLocation = item.NumberOfCity,
                            NameLocation = findInformation.City + '_' + findInformation.Province,
                            ImgLocation = findInformation.ImgPath,
                            Order = countIncrement,
                        };
                        listRecommend.Add(recommend);

                    }
                    await _recommendRepository.SaveAsync();
                }
                else
                {
                    foreach (var item in getRecommendLocation.OrderBy(x => x.Order).ToList())
                    {
                        int numberParse;
                        if (Int32.TryParse(item.IdType, out numberParse))
                        {
                            var findInformation = await _locationRepository.FindByIdAsync(numberParse);
                            listRecommend.Add(new LocationRecommendationViewModel()
                            {
                                IdLocation = findInformation.Id,
                                NameLocation = findInformation.City + '_' + findInformation.Province,
                                ImgLocation = findInformation.ImgPath,
                                Id = item.Id,
                                Order = item.Order
                            }) ;
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