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
            
            var comment = new ServiceComment{
                ContentOfRating = addComment.ContentOfRating,
                UserId = Guid.Parse(addComment.UserId),
                ServiceId = Guid.Parse(addComment.ServiceId),
                ParentId = Guid.Parse(addComment.ParentId)
            };

            _commentRepository.Add(comment);
            _commentRepository.SaveAsync();
            return new CommandResult<CommentViewModel>
                {
                    isValid = true,
                };

       
        }
    }
}