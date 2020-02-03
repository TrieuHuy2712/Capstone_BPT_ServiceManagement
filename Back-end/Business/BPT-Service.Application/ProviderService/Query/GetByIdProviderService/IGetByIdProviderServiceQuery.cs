using System;
using System.Threading.Tasks;
using BPT_Service.Application.ProviderService.ViewModel;
using BPT_Service.Model.Entities;

namespace BPT_Service.Application.ProviderService.Query.GetByIdProviderService
{
    public interface IGetByIdProviderServiceQuery
    {
         Task<CommandResult<ProviderServiceViewModel>> ExecuteAsync(Guid id);
    }
}