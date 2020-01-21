using System.Collections.Generic;
using System.Threading.Tasks;
using BPT_Service.Application.RoleService.ViewModel;

namespace BPT_Service.Application.RoleService.Query.GetAllAsync
{
    public interface IGetAllRoleAsyncQuery
    {
         Task<List<AppRoleViewModel>> ExecuteAsync();
    }
}