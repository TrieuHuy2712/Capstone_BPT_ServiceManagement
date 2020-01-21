using System.Threading.Tasks;
using BPT_Service.Application.UserService.ViewModel;

namespace BPT_Service.Application.UserService.Query.GetByIdAsync
{
    public interface IGetByIdUserAsyncQuery
    {
          Task<AppUserViewModelinUserService> ExcecuteAsync(string id);
    }
}