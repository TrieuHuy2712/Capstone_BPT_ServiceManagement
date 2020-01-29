using System;
using System.Threading.Tasks;
using BPT_Service.Model.Entities;
using BPT_Service.Model.Entities.ServiceModel;

namespace BPT_Service.Application.ProviderService.Command.DeleteProviderService
{
    public interface IDeleteProviderServiceCommand
    {
         Task<CommandResult<Provider>> ExecuteAsync(Guid id);
    }
}