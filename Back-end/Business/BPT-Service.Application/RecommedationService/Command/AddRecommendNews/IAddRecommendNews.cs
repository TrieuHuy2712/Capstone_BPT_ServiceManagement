using BPT_Service.Application.RecommedationService.ViewModel;
using BPT_Service.Model.Entities;
using System.Threading.Tasks;

namespace BPT_Service.Application.RecommedationService.Command.RecommendNews.AddRecommendNews
{
    public interface IAddRecommendNews
    {
        Task<CommandResult<NewsRecommendationViewModel>> ExecuteAsync(NewsRecommendationViewModel vm);
    }
}