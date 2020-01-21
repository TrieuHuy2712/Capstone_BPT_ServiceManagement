using System.Threading.Tasks;
using BPT_Service.Model.Entities;

namespace BPT_Service.Application.AuthenticateService.Query.AuthenticateofAuthenticationService
{
    public interface IAuthenticateServiceQuery
    {
         Task<AppUser> ExecuteAsync(string username, string password);
    }
}