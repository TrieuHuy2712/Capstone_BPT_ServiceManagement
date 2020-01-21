using System.Threading.Tasks;
using BPT_Service.Application.TagService.ViewModel;

namespace BPT_Service.Application.TagService.Command.AddServiceAsync
{
    public interface IAddTagServiceAsyncCommand
    {
         Task<bool> ExecuteAsync(TagViewModel userVm);
    }
}