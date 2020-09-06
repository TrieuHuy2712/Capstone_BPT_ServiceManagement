using System.Threading.Tasks;

namespace BPT_Service.Application.ElasticSearchService.Command.AddAllService
{
    public interface IAddAllService
    {
        Task<bool> ExecuteAsync();
    }
}