using System;
using System.Threading.Tasks;
using BPT_Service.Application.CommentService.ViewModel;
using BPT_Service.Model.Entities;
using BPT_Service.Model.Entities.ServiceModel;
using BPT_Service.Model.Infrastructure.Interfaces;

namespace BPT_Service.Application.CommentService.Command.UpdateCommentServiceAsync
{
    public class UpdateCommentServiceAsyncCommand : IUpdateCommentServiceAsyncCommand
    {
        private readonly IRepository<ServiceComment, Guid> _commentRepository;
        public UpdateCommentServiceAsyncCommand(IRepository<ServiceComment, Guid> commentRepository)
        {
            _commentRepository = commentRepository;
        }
        public async Task<CommandResult<CommentViewModel>> ExecuteAsync(CommentViewModel commentserviceVm)
        {
            try
            {
                var CommentUpdate = await _commentRepository.FindByIdAsync(Guid.Parse(commentserviceVm.Id));
                if (CommentUpdate != null)
                {
                    CommentUpdate.Id = Guid.Parse(commentserviceVm.Id);
                    CommentUpdate.ContentOfRating = commentserviceVm.ContentOfRating ;
                    CommentUpdate.UserId = Guid.Parse(commentserviceVm.UserId);
                    CommentUpdate.ServiceId = Guid.Parse(commentserviceVm.ServiceId);

                    _commentRepository.Update(CommentUpdate);
                    await _commentRepository.SaveAsync();
                    return new CommandResult<CommentViewModel>
                    {
                        isValid = true,
                        myModel = new CommentViewModel
                        {
                            Id = CommentUpdate.Id.ToString(),
                            UserId = CommentUpdate.UserId.ToString(),
                            ServiceId = CommentUpdate.ServiceId.ToString(),
                            ContentOfRating = CommentUpdate.ContentOfRating
                        }
                    };
                }
                else
                {
                    return new CommandResult<CommentViewModel>
                    {
                        isValid = false,
                        errorMessage = "Cannot find Tag"
                    };
                }
            }
            catch (System.Exception ex)
            {
                return new CommandResult<CommentViewModel>
                {
                    isValid = false,
                    errorMessage = ex.InnerException.ToString()
                };
            }
        }
    }
}