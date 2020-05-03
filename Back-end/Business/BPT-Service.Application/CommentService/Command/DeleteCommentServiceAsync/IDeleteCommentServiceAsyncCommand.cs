using System;
using System.Threading.Tasks;
using BPT_Service.Application.CommentService.ViewModel;
using BPT_Service.Model.Entities;

namespace BPT_Service.Application.CommentService.Command.DeleteCommentServiceAsync
{
    public interface IDeleteCommentServiceAsyncCommand
    {
        Task<CommandResult<CommentViewModel>> ExecuteAsync(int id);
    }
}