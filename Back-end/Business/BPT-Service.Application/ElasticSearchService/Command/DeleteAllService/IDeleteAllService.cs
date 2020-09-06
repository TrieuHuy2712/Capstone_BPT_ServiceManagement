using System.Threading.Tasks;

namespace BPT_Service.Application.ElasticSearchService.Command.DeleteAllService
{
    public interface IDeleteAllService
    {
        Task<bool> ExecuteAsync();
    }
}