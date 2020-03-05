using System.Threading.Tasks;
using BPT_Service.Application.NewsProviderService.ViewModel;
using BPT_Service.Model.Entities;

namespace BPT_Service.Application.NewsProviderService.Command.ApproveNewsProvider
{
    public interface IApproveNewsProviderServiceCommand
    {
         Task<CommandResult<NewsProviderViewModel>> ExecuteAsync(int idNews);
    }
}