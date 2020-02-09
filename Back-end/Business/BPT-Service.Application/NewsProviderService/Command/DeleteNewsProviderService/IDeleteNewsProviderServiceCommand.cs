using System.Threading.Tasks;
using BPT_Service.Model.Entities;
using BPT_Service.Model.Entities.ServiceModel.ProviderServiceModel;

namespace BPT_Service.Application.NewsProviderService.Command.DeleteNewsProviderService
{
    public interface IDeleteNewsProviderServiceCommand
    {
        Task<CommandResult<ProviderNew>> ExecuteAsync(int id);
    }
}