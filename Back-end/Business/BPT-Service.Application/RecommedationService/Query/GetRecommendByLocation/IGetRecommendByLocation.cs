using BPT_Service.Application.RecommedationService.ViewModel;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BPT_Service.Application.RecommedationService.Query.GetRecommendByLocation
{
    public interface IGetRecommendByLocation
    {
        Task<List<LocationRecommendationViewModel>> ExecuteAsync(bool isSetDefault);
    }
}