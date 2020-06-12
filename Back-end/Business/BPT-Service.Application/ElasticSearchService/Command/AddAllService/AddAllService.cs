using BPT_Service.Application.ElasticSearchService.Command.DeleteAllService;
using BPT_Service.Application.PostService.Query.GetAllPagingPostService;
using BPT_Service.Application.PostService.ViewModel;
using BPT_Service.Model.IRepositories;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace BPT_Service.Application.ElasticSearchService.Command.AddAllService
{
    public class AddAllService : IAddAllService
    {
        private readonly IGetAllPagingPostServiceQuery _getAllPagingPostServiceQuery;
        private readonly IElasticSearchRepository<PostServiceViewModel> _elasticSearchRepository;
        private readonly IDeleteAllService _deleteAllService;

        public AddAllService(
            IGetAllPagingPostServiceQuery getAllPagingPostServiceQuery,
            IElasticSearchRepository<PostServiceViewModel> elasticSearchRepository,
            IDeleteAllService deleteAllService)
        {
            _getAllPagingPostServiceQuery = getAllPagingPostServiceQuery;
            _elasticSearchRepository = elasticSearchRepository;
            _deleteAllService = deleteAllService;
        }

        public async Task<bool> ExecuteAsync()
        {
            try
            {
                // Delete All Service
                await _deleteAllService.ExecuteAsync();

                //Get All Service
                var pagingService = await _getAllPagingPostServiceQuery.ExecuteAsync(string.Empty, 1, 0, false, 0);
                var listService = pagingService.Results;
                await _elasticSearchRepository.SaveManyAsync(listService.ToArray());
                return true;
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}