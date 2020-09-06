using BPT_Service.Application.RatingService.ViewModel;
using System.Threading.Tasks;

namespace BPT_Service.Application.RatingService.Query.GetRatingByService
{
    public interface IGetRatingByService
    {
        Task<ListRatingByServiceViewModel> ExecuteAsync(string idService);
    }
}