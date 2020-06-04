using BPT_Service.Application.PostService.ViewModel;
using BPT_Service.Common.Helpers;
using BPT_Service.Common.Logging;
using BPT_Service.Model.IRepositories;
using System;
using System.Threading.Tasks;

namespace BPT_Service.Application.ElasticSearchService.Command.AddNewService
{
    public class AddNewService : IAddNewService
    {
        private readonly IElasticSearchRepository<PostServiceViewModel> _elasticSearchRepository;

        public AddNewService(IElasticSearchRepository<PostServiceViewModel> elasticSearchRepository)
        {
            _elasticSearchRepository = elasticSearchRepository;
        }

        public async Task<PostServiceViewModel> ExecuteAsync(PostServiceViewModel postService)
        {
            try
            {
                await _elasticSearchRepository.SaveSingleAsync(postService);
                return postService;
            }
            catch (Exception ex)
            {
                await Logging<AddNewService>.ErrorAsync(ex, ActionCommand.COMMAND_ADD, "System", "Has error");
                throw;
            }
        }
    }
}