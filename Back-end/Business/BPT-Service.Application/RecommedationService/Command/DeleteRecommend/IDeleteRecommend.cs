using BPT_Service.Application.RecommedationService.ViewModel;
using BPT_Service.Model.Entities;
using System.Threading.Tasks;

namespace BPT_Service.Application.RecommedationService.Command.RecommendLocation.DeleteRecommendLocation
{
    public interface IDeleteRecommend
    {
        Task<CommandResult<LocationRecommendationViewModel>> ExecuteAsync(int id);
    }
}