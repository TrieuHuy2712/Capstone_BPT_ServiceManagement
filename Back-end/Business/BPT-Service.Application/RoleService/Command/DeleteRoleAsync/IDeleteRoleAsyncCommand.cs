using System;
using System.Threading.Tasks;

namespace BPT_Service.Application.RoleService.Command.DeleteRoleAsync
{
    public interface IDeleteRoleAsyncCommand
    {
         Task ExecuteAsync(Guid id);
    }
}