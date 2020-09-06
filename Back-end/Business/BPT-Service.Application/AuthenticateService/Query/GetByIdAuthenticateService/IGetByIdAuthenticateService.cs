using BPT_Service.Application.AuthenticateService.ViewModel;
using System.Threading.Tasks;

namespace BPT_Service.Application.AuthenticateService.Query.GetByIdAuthenticateService
{
    public interface IGetByIdAuthenticateService
    {
        Task<AppUserViewModel> ExecuteAsync(string id);
    }
}