using BPT_Service.Application.PostService.ViewModel;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BPT_Service.Application.ElasticSearchService.Query
{
    public interface ISearchService
    {
        Task<IEnumerable<PostServiceViewModel>> ExecuteAsync(string query, int page = 1, int pageSize = 5);
    }
}