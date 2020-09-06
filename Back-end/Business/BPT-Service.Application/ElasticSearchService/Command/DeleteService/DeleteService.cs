using BPT_Service.Application.PostService.ViewModel;
using BPT_Service.Common.Helpers;
using BPT_Service.Common.Logging;
using BPT_Service.Model.IRepositories;
using System;
using System.Threading.Tasks;

namespace BPT_Service.Application.ElasticSearchService.Command.DeleteService
{
    public class DeleteService : IDeleteService
    {
        private readonly IElasticSearchRepository<PostServiceViewModel> _elasticSearchRepository;

        public DeleteService(IElasticSearchRepository<PostServiceViewModel> elasticSearchRepository)
        {
            _elasticSearchRepository = elasticSearchRepository;
        }

        public async Task<PostServiceViewModel> ExecuteAsync(PostServiceViewModel model)
        {
            try
            {
                await _elasticSearchRepository.DeleteAsync(model);
                return model;
            }
            catch (Exception ex)
            {
                await Logging<DeleteService>.ErrorAsync(ex, ActionCommand.COMMAND_DELETE, "System", "Has error");
                return null;
            }
        }
    }
}