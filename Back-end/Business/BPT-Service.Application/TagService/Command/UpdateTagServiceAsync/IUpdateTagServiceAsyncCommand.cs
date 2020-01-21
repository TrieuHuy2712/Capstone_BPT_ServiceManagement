using System.Threading.Tasks;
using BPT_Service.Application.TagService.ViewModel;

namespace BPT_Service.Application.TagService.Command.UpdateTagServiceAsync
{
    public interface IUpdateTagServiceAsyncCommand
    {
          Task<bool> ExecuteAsync(TagViewModel userVm);
    }
}