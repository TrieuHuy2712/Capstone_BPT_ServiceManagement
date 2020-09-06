using BPT_Service.Application.PostService.ViewModel;
using BPT_Service.Common.Dtos;
using System.Threading.Tasks;

namespace BPT_Service.Application.PostService.Query.FilterAllPagingPostService
{
    public interface IFilterAllPagingPostServiceQuery
    {
        Task<PagedResult<ListServiceViewModel>> ExecuteAsync(int page, int pageSize, string typeFilter, string filterName);
    }
}