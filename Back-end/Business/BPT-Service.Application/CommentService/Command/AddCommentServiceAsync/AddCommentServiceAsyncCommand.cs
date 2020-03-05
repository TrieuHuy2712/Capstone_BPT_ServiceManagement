using System;
using System.Threading.Tasks;
using BPT_Service.Application.CommentService.ViewModel;
using BPT_Service.Model.Entities;
using BPT_Service.Model.Entities.ServiceModel;
using BPT_Service.Model.Infrastructure.Interfaces;

namespace BPT_Service.Application.CommentService.Command.AddCommentServiceAsync
{
    public class AddCommentServiceAsyncCommand : IAddCommentServiceAsyncCommand
    {
        private readonly IRepository<ServiceComment, Guid> _commentRepository;
        public AddCommentServiceAsyncCommand(IRepository<ServiceComment, Guid> commentRepository)
        {
            _commentRepository = commentRepository;
        }
        public async Task<CommandResult<CommentViewModel>> ExecuteAsync(CommentViewModel addComment)
        {
            
            ServiceComment comment = new ServiceComment();
            addComment.ContentOfRating = comment.ContentOfRating;
            addComment.ParentId = comment.ParentId.ToString();
            addComment.UserId = comment.UserId.ToString();
            addComment.ServiceId = comment.ServiceId.ToString();

            await _commentRepository.Add(comment);
            await _commentRepository.SaveAsync();
            return new CommandResult<CommentViewModel>
                {
                    isValid = true,
                };

       
        }
    }
}