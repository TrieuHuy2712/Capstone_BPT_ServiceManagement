using System.Collections.Generic;
using System.Threading.Tasks;
using BPT_Service.Application.FunctionService.ViewModel;

namespace BPT_Service.Application.FunctionService.Query.GetListFunctionWithPermission
{
    public interface IGetListFunctionWithPermissionQuery
    {
         Task<List<FunctionViewModelinFunctionService>> ExecuteAsync(string userName);
    }
}