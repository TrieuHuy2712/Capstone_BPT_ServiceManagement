using BPT_Service.Application.RoleService.ViewModel;
using BPT_Service.Common.Dtos;

namespace BPT_Service.Application.RoleService.Query.GetAllPagingAsync
{
    public interface IGetAllRolePagingAsyncQuery
    {
         PagedResult<AppRoleViewModel> ExecuteAsync(string keyword, int page, int pageSize);
    }
}