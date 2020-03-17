using System.Threading.Tasks;
using BPT_Service.Application.CommentService.ViewModel;
using BPT_Service.Model.Entities;

namespace BPT_Service.Application.CommentService.Command.AddCommentServiceAsync
{
    public interface IAddCommentServiceAsyncCommand
    {
         Task<CommandResult<CommentViewModel>> ExecuteAsync(CommentViewModel addComment);
    }
}