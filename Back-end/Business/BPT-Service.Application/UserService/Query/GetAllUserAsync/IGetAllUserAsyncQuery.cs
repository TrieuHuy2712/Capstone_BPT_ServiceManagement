using System.Collections.Generic;
using System.Threading.Tasks;
using BPT_Service.Application.UserService.ViewModel;

namespace BPT_Service.Application.UserService.Query.GetAllAsync
{
    public interface IGetAllUserAsyncQuery
    {
        Task<List<AppUserViewModelinUserService>> ExecuteAsync();
    }
}