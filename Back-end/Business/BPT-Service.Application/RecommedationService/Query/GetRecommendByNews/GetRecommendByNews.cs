using BPT_Service.Application.RecommedationService.ViewModel;
using BPT_Service.Model.Entities;
using BPT_Service.Model.Entities.ServiceModel.ProviderServiceModel;
using BPT_Service.Model.Enums;
using BPT_Service.Model.Infrastructure.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BPT_Service.Application.RecommedationService.Query.GetRecommendByNews
{
    public class GetRecommendByNews : IGetRecommendByNews
    {
        private readonly IRepository<Recommendation, int> _recommendRepository;
        private readonly IRepository<ProviderNew, int> _providerNewsRepository;

        public GetRecommendByNews(IRepository<Recommendation, int> recommendRepository,
            IRepository<ProviderNew, int> providerNewsRepository)
        {
            _recommendRepository = recommendRepository;
            _providerNewsRepository = providerNewsRepository;
        }

        public async Task<List<NewsRecommendationViewModel>> ExecuteAsync(bool isSetDefault)
        {
            try
            {
                var getRecommendLocation = await _recommendRepository.FindAllAsync(x => x.Type == TypeRecommendation.News);
                List<NewsRecommendationViewModel> listRecommend = new List<NewsRecommendationViewModel>();
                if (isSetDefault)
                {
                    var getNewsRecommendation = await _providerNewsRepository.FindAllAsync();
                    var getTopTenNews = getNewsRecommendation.OrderByDescending(x => x.DateCreated).Take(10).ToList();

                    //Add new Information
                    var countIncrement = 0;
                    foreach (var item in getTopTenNews)
                    {
                        var addRecommend = new Recommendation()
                        {
                            IdType = item.Id.ToString(),
                            Order = ++countIncrement,
                            Type = TypeRecommendation.Location
                        };
                        await _recommendRepository.Add(addRecommend);

                        var recommend = new NewsRecommendationViewModel()
                        {
                            Id = addRecommend.Id,
                            IdNews = item.Id,
                            ImgNews = item.ImgPath,
                            Order = countIncrement,
                            TitleNews = item.Title
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
                            var findInformation = await _providerNewsRepository.FindByIdAsync(numberParse);
                            listRecommend.Add(new NewsRecommendationViewModel()
                            {
                                Id = item.Id,
                                IdNews = findInformation.Id,
                                ImgNews = findInformation.ImgPath,
                                TitleNews = findInformation.Title,
                                Order = item.Order
                            }); ;
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