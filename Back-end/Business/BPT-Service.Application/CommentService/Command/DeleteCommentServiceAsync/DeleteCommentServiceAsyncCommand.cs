using System;
using System.Threading.Tasks;
using BPT_Service.Application.CommentService.ViewModel;
using BPT_Service.Model.Entities;
using BPT_Service.Model.Entities.ServiceModel;
using BPT_Service.Model.Infrastructure.Interfaces;

namespace BPT_Service.Application.CommentService.Command.DeleteCommentServiceAsync
{
    public class DeleteCommentServiceAsyncCommand : IDeleteCommentServiceAsyncCommand
    {
        private readonly IRepository<ServiceComment, Guid> _commentRepository;

        public DeleteCommentServiceAsyncCommand(IRepository<ServiceComment, Guid> commentRepository)
        {
            _commentRepository = commentRepository;
        }
        public async Task<CommandResult<CommentViewModel>> ExecuteAsync(Guid id)
        {
            try
            {
                var commentDel = await _commentRepository.FindByIdAsync(id);
                if (commentDel != null)
                {
                    _commentRepository.Remove(commentDel);
                    await _commentRepository.SaveAsync();
                    return new CommandResult<CommentViewModel>
                    {
                        isValid = true,
                        myModel = new CommentViewModel
                        {
                            Id = commentDel.Id.ToString(),
                        }
                    };
                }
                else
                {
                    return new CommandResult<CommentViewModel>
                    {
                        isValid = false,
                        errorMessage = "Cannot find Tag ID"
                    };
                }
            }
            catch (System.Exception ex)
            {
                return new CommandResult<CommentViewModel>
                {
                    isValid = true,
                    errorMessage = ex.InnerException.ToString()
                };
            }
        }
    }
}