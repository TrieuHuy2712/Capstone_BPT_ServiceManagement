using System.Threading.Tasks;
using BPT_Service.Application.NewsProviderService.ViewModel;
using BPT_Service.Model.Entities;

namespace BPT_Service.Application.NewsProviderService.Command.UpdateNewsProviderService
{
    public interface IUpdateNewsProviderServiceCommand
    {
         Task<CommandResult<NewsProviderViewModel>> ExecuteAsync(NewsProviderViewModel vm);
    }
}