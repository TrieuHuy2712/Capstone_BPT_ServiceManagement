using System.Threading.Tasks;
using BPT_Service.Application.NewsProviderService.ViewModel;
using BPT_Service.Model.Entities;

namespace BPT_Service.Application.NewsProviderService.Command.RegisterNewsProviderService
{
    public interface IRegisterNewsProviderServiceCommand
    {
        Task<CommandResult<NewsProviderViewModel>> ExecuteAsync(NewsProviderViewModel vm);
    }
}