using System.Collections.Generic;
using System.Threading.Tasks;
using BPT_Service.Application.AuthenticateService.ViewModel;

namespace BPT_Service.Application.AuthenticateService.Query.GetAllAuthenticateService
{
    public interface IGetAllAuthenticateServiceQuery
    {
        Task<IEnumerable<AppUserViewModel>> ExecuteAsync(); 
    }
}