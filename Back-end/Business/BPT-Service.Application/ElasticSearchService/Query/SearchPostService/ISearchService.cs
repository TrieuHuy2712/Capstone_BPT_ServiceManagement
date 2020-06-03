using BPT_Service.Application.PostService.ViewModel;
using Nest;
using System.Threading.Tasks;

namespace BPT_Service.Application.ElasticSearchService.Query
{
    public interface ISearchService
    {
        Task<ISearchResponse<PostServiceViewModel>> ExecuteAsync(string query, int page = 1, int pageSize = 5);
    }
}