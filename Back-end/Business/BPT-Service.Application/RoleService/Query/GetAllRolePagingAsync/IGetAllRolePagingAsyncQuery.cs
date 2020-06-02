using BPT_Service.Application.RoleService.ViewModel;
using BPT_Service.Common.Dtos;
using System.Threading.Tasks;

namespace BPT_Service.Application.RoleService.Query.GetAllPagingAsync
{
    public interface IGetAllRolePagingAsyncQuery
    {
         Task<PagedResult<AppRoleViewModel>> ExecuteAsync(string keyword, int page, int pageSize);
    }
}