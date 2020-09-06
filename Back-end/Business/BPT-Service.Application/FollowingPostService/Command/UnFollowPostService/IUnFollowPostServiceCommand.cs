using BPT_Service.Application.FollowingPostService.ViewModel;
using BPT_Service.Model.Entities;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace BPT_Service.Application.FollowingPostService.Command.UnFollowPostService
{
    public interface IUnFollowPostServiceCommand
    {
        Task<CommandResult<ServiceFollowingViewModel>> ExecuteAsync(ServiceFollowingViewModel model);
    }
}
