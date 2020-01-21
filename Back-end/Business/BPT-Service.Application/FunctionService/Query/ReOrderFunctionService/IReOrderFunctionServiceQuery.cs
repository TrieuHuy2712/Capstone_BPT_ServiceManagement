using System.Threading.Tasks;

namespace BPT_Service.Application.FunctionService.Query.ReOrderFunctionService
{
    public interface IReOrderFunctionServiceQuery
    {
        Task<bool> ExecuteAsync(string sourceId, string targetId);
    }
}