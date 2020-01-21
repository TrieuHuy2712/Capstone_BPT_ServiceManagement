using System.Threading.Tasks;
using BPT_Service.Application.UserService.ViewModel;

namespace BPT_Service.Application.UserService.Command.AddCustomerAsync
{
    public interface IAddCustomerAsyncCommand
    {
         Task<bool> ExecuteAsync(AppUserViewModelinUserService userVm, string password);
    }
}