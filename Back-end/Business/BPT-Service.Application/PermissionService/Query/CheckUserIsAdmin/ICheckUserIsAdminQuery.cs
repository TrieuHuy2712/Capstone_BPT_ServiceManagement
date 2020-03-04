using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace BPT_Service.Application.PermissionService.Query.CheckUserIsAdmin
{
    public interface ICheckUserIsAdminQuery
    {
        Task<bool> ExecuteAsync(string userId);
    }
}
