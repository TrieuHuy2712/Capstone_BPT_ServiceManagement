using BPT_Service.Application.RecommedationService.ViewModel;
using BPT_Service.Model.Entities;
using System.Threading.Tasks;

namespace BPT_Service.Application.RecommedationService.Command.RecommendLocation.AddRecommendLocation
{
    public interface IAddRecommendLocation
    {
        Task<CommandResult<LocationRecommendationViewModel>> ExecuteAsync(LocationRecommendationViewModel vm);
    }
}