using BPT_Service.Application.PostService.ViewModel;
using System.Threading.Tasks;

namespace BPT_Service.Application.ElasticSearchService.Command.AddNewService
{
    public interface IAddNewService
    {
        Task<PostServiceViewModel> ExecuteAsync(PostServiceViewModel model);
    }
}