using System.Collections.Generic;
using System.Threading.Tasks;
using BPT_Service.Application.UserService.ViewModel;
using BPT_Service.Common.Dtos;

namespace BPT_Service.Application.UserService.Query.GetAllPagingAsync
{
    public interface IGetAllPagingUserAsyncQuery
    {
         Task<PagedResult<AppUserViewModelinUserService>> ExecuteAsync(string keyword, int page, int pageSize);
    }
}