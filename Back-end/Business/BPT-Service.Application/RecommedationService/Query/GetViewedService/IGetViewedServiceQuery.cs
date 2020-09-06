using BPT_Service.Application.RecommedationService.ViewModel;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BPT_Service.Application.RecommedationService.Query.GetViewedService
{
    public interface IGetViewedServiceQuery
    {
        Task<List<ServiceRecommendationViewModel>> ExecuteAsync();
    }
}