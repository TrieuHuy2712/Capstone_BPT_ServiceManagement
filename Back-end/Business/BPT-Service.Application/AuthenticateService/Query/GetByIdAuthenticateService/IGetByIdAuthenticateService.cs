using System.Threading.Tasks;
using BPT_Service.Application.AuthenticateService.ViewModel;

namespace BPT_Service.Application.AuthenticateService.Query.GetByIdAuthenticateService
{
    public interface IGetByIdAuthenticateService
    {
         Task<AppUserViewModel> ExecuteAsync(string id);
    }
}