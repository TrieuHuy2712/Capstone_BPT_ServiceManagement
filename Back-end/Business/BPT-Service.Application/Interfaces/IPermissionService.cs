using System.Threading.Tasks;
using BPT_Service.Application.ViewModels.System;

namespace BPT_Service.Application.Interfaces
{
    public interface IPermissionService
    {
        Task<PermissionSingleViewModel> GetPermissionRole(string userName, string functionId);
    }
}