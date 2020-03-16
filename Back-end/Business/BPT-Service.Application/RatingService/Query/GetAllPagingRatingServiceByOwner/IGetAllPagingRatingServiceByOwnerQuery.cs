using BPT_Service.Application.RatingService.ViewModel;
using BPT_Service.Common.Dtos;
using System.Threading.Tasks;

namespace BPT_Service.Application.RatingService.Query.GetAllPagingRatingServiceByOwner
{
    public interface IGetAllPagingRatingServiceByOwnerQuery
    {
        Task<PagedResult<ServiceRatingViewModel>> ExecuteAsync(string keyword, int page, int pageSize, string idService);
    }
}