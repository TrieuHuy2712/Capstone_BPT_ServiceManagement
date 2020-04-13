using System.Threading.Tasks;
using BPT_Service.Application.PostService.ViewModel;
using BPT_Service.Common.Dtos;

namespace BPT_Service.Application.PostService.Query.GetAllPagingPostService
{
    public interface IGetAllPagingPostServiceQuery
    {
          Task<PagedResult<PostServiceViewModel>> ExecuteAsync(string keyword, int page, int pageSize, bool isAdminPage, int filter);
    }
}