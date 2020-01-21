using System.Collections.Generic;
using System.Threading.Tasks;
using BPT_Service.Application.FunctionService.ViewModel;

namespace BPT_Service.Application.FunctionService.Query.GetAllFunctionService
{
    public interface IGetAllFunctionServiceQuery
    {
        Task<List<FunctionViewModelinFunctionService>> ExecuteAsync(string filter);
    }
}