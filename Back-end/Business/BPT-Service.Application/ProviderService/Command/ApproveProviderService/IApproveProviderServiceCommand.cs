using System.Threading.Tasks;
using BPT_Service.Application.ProviderService.ViewModel;
using BPT_Service.Model.Entities;

namespace BPT_Service.Application.ProviderService.Command.ApproveProviderService
{
    public interface IApproveProviderServiceCommand
    {
         Task<CommandResult<ProviderServiceViewModel>> ExecuteAsync(ProviderServiceViewModel vm);
    }
}