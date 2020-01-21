using System.Threading.Tasks;

namespace BPT_Service.Application.FunctionService.Query.CheckExistedIdFunctionService
{
    public interface ICheckExistedFunctionServiceQuery
    {
        Task<bool> ExecuteAsync(string id);
    }
}