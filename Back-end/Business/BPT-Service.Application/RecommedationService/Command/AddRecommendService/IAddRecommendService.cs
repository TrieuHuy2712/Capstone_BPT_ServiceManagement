using BPT_Service.Application.RecommedationService.ViewModel;
using BPT_Service.Model.Entities;
using System.Threading.Tasks;

namespace BPT_Service.Application.RecommedationService.Command.AddRecommendService
{
    public interface IAddRecommendService
    {
        Task<CommandResult<AddRecommendationViewModel>> ExecuteAsync(AddRecommendationViewModel vm);
    }
}