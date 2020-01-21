using System.Threading.Tasks;
using BPT_Service.Application.FunctionService.ViewModel;

namespace BPT_Service.Application.FunctionService.Query.GetByIdFunctionService
{
    public interface IGetByIdFunctionServiceQuery
    {
        Task<FunctionViewModelinFunctionService> ExecuteAsync(string id);
    }
}