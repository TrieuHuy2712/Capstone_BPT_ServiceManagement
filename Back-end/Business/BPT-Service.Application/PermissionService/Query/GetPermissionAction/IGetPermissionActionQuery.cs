using System;
using System.Threading.Tasks;

namespace BPT_Service.Application.PermissionService.Query.GetPermissionAction
{
    public interface IGetPermissionActionQuery
    {
        Task<bool> ExecuteAsync(string userId, string functionId, string action);
    }
}