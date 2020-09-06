using BPT_Service.Application.RecommedationService.ViewModel;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BPT_Service.Application.RecommedationService.Query.GetRecommendByNews
{
    public interface IGetRecommendByNews
    {
        Task<List<NewsRecommendationViewModel>> ExecuteAsync(bool isSetDefault);
    }
}