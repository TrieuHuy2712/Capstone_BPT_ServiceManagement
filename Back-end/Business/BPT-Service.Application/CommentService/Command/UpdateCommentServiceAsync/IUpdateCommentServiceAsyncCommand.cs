using System.Threading.Tasks;
using BPT_Service.Application.CommentService.ViewModel;
using BPT_Service.Model.Entities;

namespace BPT_Service.Application.CommentService.Command.UpdateCommentServiceAsync
{
    public interface IUpdateCommentServiceAsyncCommand
    {
        Task<CommandResult<CommentViewModel>> ExecuteAsync(CommentViewModel commentserviceVm);
    }
}