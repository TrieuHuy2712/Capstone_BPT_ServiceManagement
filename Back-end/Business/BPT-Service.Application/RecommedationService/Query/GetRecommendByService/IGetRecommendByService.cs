using BPT_Service.Application.RecommedationService.ViewModel;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BPT_Service.Application.RecommedationService.Query.GetRecommendByService
{
    public interface IGetRecommendByService
    {
        Task<List<ServiceRecommendationViewModel>> ExecuteAsync(bool isSetDefault);
    }
}