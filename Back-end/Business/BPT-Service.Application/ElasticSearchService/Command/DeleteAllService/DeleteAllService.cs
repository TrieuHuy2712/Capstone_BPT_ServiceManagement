using BPT_Service.Application.PostService.ViewModel;
using BPT_Service.Model.IRepositories;
using System;
using System.Threading.Tasks;

namespace BPT_Service.Application.ElasticSearchService.Command.DeleteAllService
{
    public class DeleteAllService : IDeleteAllService
    {
        private readonly IElasticSearchRepository<PostServiceViewModel> _elasticSearchRepository;

        public DeleteAllService(IElasticSearchRepository<PostServiceViewModel> elasticSearchRepository)
        {
            _elasticSearchRepository = elasticSearchRepository;
        }

        public async Task<bool> ExecuteAsync()
        {
            try
            {
                await _elasticSearchRepository.DeleteAllAsync();
                return true;
            }
            catch (Exception)
            {

                throw;
            }
        }
    }
}