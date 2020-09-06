using BPT_Service.Application.PostService.ViewModel;
using System.Threading.Tasks;

namespace BPT_Service.Application.ElasticSearchService.Command.DeleteService
{
    public interface IDeleteService
    {
        Task<PostServiceViewModel> ExecuteAsync(PostServiceViewModel model);
    }
}