using System.Threading.Tasks;

namespace BPT_Service.Application.AuthenticateService.Query.CheckCanAccessMain
{
    public interface ICheckCanAccessMain
    {
        Task<bool> ExecuteAsync(string userName);
    }
}