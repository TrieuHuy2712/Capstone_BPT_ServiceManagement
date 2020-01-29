using System;
using System.Threading.Tasks;
using BPT_Service.Application.TagService.ViewModel;
using BPT_Service.Model.Entities;

namespace BPT_Service.Application.TagService.Command.DeleteServiceAsync
{
    public interface IDeleteTagServiceAsyncCommand
    {
        Task<CommandResult<TagViewModel>> ExecuteAsync(Guid id);
    }
}