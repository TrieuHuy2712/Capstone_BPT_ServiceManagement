using System.Threading.Tasks;
using BPT_Service.Application.ProviderService.ViewModel;
using BPT_Service.Model.Entities;

namespace BPT_Service.Application.ProviderService.Query.CheckUserIsProvider
{
    public interface ICheckUserIsProviderQuery
    {
        Task<CommandResult<ProviderServiceViewModel>> ExecuteAsync();
    }
}