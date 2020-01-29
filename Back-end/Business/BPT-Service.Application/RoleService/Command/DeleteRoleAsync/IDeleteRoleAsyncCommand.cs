using System;
using System.Threading.Tasks;
using BPT_Service.Application.RoleService.ViewModel;
using BPT_Service.Model.Entities;

namespace BPT_Service.Application.RoleService.Command.DeleteRoleAsync
{
    public interface IDeleteRoleAsyncCommand
    {
         Task<CommandResult<AppRoleViewModel>> ExecuteAsync(Guid id);
    }
}