using BPT_Service.Application.PostService.ViewModel;
using Nest;
using System.Threading.Tasks;

namespace BPT_Service.Application.ElasticSearchService.Query.SearchPostService
{
    public class SearchService : ISearchService
    {
        private readonly IElasticClient _elasticClient;

        public SearchService(IElasticClient elasticClient)
        {
            _elasticClient = elasticClient;
        }

        public async Task<ISearchResponse<PostServiceViewModel>> ExecuteAsync(string query, int page = 1, int pageSize = 5)
        {
            var response = await _elasticClient.SearchAsync<PostServiceViewModel>(
                             s => s.Query(q => q.QueryString(d => d.Query('*' + query + '*')))
                                 .From((page - 1) * pageSize)
                                 .Size(pageSize));
            return response;

        }
    }
}