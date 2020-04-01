using BPT_Service.Model.Entities;
using System.Threading.Tasks;

namespace BPT_Service.Application.AuthenticateService.Query.AuthenticateofAuthenticationService
{
    public interface IAuthenticateServiceQuery
    {
        Task<AppUser> ExecuteAsync(string username, string password);
    }
}