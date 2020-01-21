using System.Threading.Tasks;
using BPT_Service.Application.TagService.ViewModel;
using BPT_Service.Common.Dtos;

namespace BPT_Service.Application.TagService.Query.GetAllPagingServiceAsync
{
    public interface IGetAllPagingTagServiceAsyncQuery
    {
        Task<PagedResult<TagViewModel>> ExecuteAsync(string keyword, int page, int pageSize);
    }
}