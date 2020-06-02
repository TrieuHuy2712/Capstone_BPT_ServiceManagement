using BPT_Service.Application.RecommedationService.ViewModel;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BPT_Service.Application.RecommedationService.Query.RecommendUserService
{
    public interface IRecommendUserService
    {
        Task<List<ServiceRecommendationViewModel>> ExecuteAsync();
    }
}