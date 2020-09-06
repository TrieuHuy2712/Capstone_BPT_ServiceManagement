using BPT_Service.Application.FollowingProviderService.ViewModel;
using BPT_Service.Model.Entities;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace BPT_Service.Application.FollowingProviderService.Command.RegisterEmailProviderService
{
    public interface IRegisterEmailProviderServiceCommand
    {
        Task<CommandResult<FollowingProviderServiceViewModel>> ExecuteAsync(int idRegister);
    }
}
