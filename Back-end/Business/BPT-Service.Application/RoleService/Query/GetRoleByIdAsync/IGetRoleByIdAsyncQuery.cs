using System;
using System.Threading.Tasks;
using BPT_Service.Application.RoleService.ViewModel;

namespace BPT_Service.Application.RoleService.Query.GetByIdAsync
{
    public interface IGetRoleByIdAsyncQuery
    {
          Task<AppRoleViewModel> ExecuteAsync(Guid id);
    }
}