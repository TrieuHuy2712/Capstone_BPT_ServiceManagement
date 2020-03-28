using System.Threading.Tasks;
using BPT_Service.Application.NewsProviderService.ViewModel;
using BPT_Service.Model.Entities;

namespace BPT_Service.Application.NewsProviderService.Command.RejectNewsProvider
{
    public interface IRejectNewsProviderServiceCommand
    {
        Task<CommandResult<NewsProviderViewModel>> ExecuteAsync(int id, string reason);
    }
}