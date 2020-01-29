using System.Threading.Tasks;
using BPT_Service.Application.TagService.ViewModel;
using BPT_Service.Model.Entities;

namespace BPT_Service.Application.TagService.Command.UpdateTagServiceAsync
{
    public interface IUpdateTagServiceAsyncCommand
    {
        Task<CommandResult<TagViewModel>> ExecuteAsync(TagViewModel userVm);
    }
}