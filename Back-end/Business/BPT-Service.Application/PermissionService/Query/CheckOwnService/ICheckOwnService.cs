using System.Threading.Tasks;

namespace BPT_Service.Application.PermissionService.Query.CheckOwnService
{
    public interface ICheckOwnService
    {
        Task<bool> ExecuteAsync(string stringUserId, string serviceId);
    }
}