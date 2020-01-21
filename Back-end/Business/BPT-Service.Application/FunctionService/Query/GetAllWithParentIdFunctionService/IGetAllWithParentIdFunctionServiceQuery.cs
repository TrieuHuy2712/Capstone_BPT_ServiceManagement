using System.Collections.Generic;
using System.Threading.Tasks;
using BPT_Service.Application.FunctionService.ViewModel;

namespace BPT_Service.Application.FunctionService.Query.GetAllWithParentIdFunctionService
{
    public interface IGetAllWithParentIdFunctionServiceQuery
    {
         Task<IEnumerable<FunctionViewModelinFunctionService>> ExecuteAsync(string parentId);
    }
}