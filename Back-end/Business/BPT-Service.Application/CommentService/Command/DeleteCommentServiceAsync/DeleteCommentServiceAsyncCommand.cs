using BPT_Service.Application.CommentService.ViewModel;
using BPT_Service.Application.PostService.Query.Extension.GetOwnServiceInformation;
using BPT_Service.Common;
using BPT_Service.Common.Helpers;
using BPT_Service.Common.Logging;
using BPT_Service.Model.Entities;
using BPT_Service.Model.Entities.ServiceModel;
using BPT_Service.Model.Infrastructure.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using System;
using System.Security.Claims;
using System.Threading.Tasks;

namespace BPT_Service.Application.CommentService.Command.DeleteCommentServiceAsync
{
    public class DeleteCommentServiceAsyncCommand : IDeleteCommentServiceAsyncCommand
    {
        private readonly IRepository<ServiceComment, Guid> _commentRepository;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IGetOwnServiceInformationQuery _getOwnServiceInformationQuery;
        private readonly UserManager<AppUser> _userManager;

        public DeleteCommentServiceAsyncCommand(
            IRepository<ServiceComment, Guid> commentRepository,
            IHttpContextAccessor httpContextAccessor,
            IGetOwnServiceInformationQuery getOwnServiceInformationQuery,
            UserManager<AppUser> userManager)
        {
            _commentRepository = commentRepository;
            _httpContextAccessor = httpContextAccessor;
            _getOwnServiceInformationQuery = getOwnServiceInformationQuery;
            _userManager = userManager;
        }

        public async Task<CommandResult<CommentViewModel>> ExecuteAsync(Guid id)
        {
            var userId = _httpContextAccessor.HttpContext.User.Identity.Name;
            var userName = _userManager.FindByIdAsync(userId).Result.UserName;
            try
            {
                var commentDel = await _commentRepository.FindByIdAsync(id);
                if (commentDel != null)
                {
                    var getUserService = await _getOwnServiceInformationQuery.ExecuteAsync(commentDel.ServiceId.ToString());
                    _commentRepository.Remove(commentDel);
                    await _commentRepository.SaveAsync();
                    await LoggingUser<DeleteCommentServiceAsyncCommand>.InformationAsync(getUserService, userName, commentDel.ContentOfRating);
                    await Logging<DeleteCommentServiceAsyncCommand>.InformationAsync(ActionCommand.COMMAND_DELETE, userName, userName + " deleted " + commentDel.ContentOfRating);
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
                    await Logging<DeleteCommentServiceAsyncCommand>.ErrorAsync(ActionCommand.COMMAND_DELETE, userName, ErrorMessageConstant.ERROR_CANNOT_FIND_ID);
                    return new CommandResult<CommentViewModel>
                    {
                        isValid = false,
                        errorMessage = ErrorMessageConstant.ERROR_CANNOT_FIND_ID
                    };
                }
            }
            catch (System.Exception ex)
            {
                await Logging<DeleteCommentServiceAsyncCommand>.ErrorAsync(ex, ActionCommand.COMMAND_DELETE, userName, "Has error");
                return new CommandResult<CommentViewModel>
                {
                    isValid = true,
                    errorMessage = ex.InnerException.ToString()
                };
            }
        }
    }
}