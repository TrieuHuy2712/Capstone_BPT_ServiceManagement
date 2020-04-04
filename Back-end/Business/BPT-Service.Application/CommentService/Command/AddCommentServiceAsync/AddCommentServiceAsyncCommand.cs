using BPT_Service.Application.CommentService.ViewModel;
using BPT_Service.Application.PostService.Query.Extension.GetOwnServiceInformation;
using BPT_Service.Common.Helpers;
using BPT_Service.Common.Logging;
using BPT_Service.Model.Entities;
using BPT_Service.Model.Entities.ServiceModel;
using BPT_Service.Model.Infrastructure.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Newtonsoft.Json;
using System;
using System.Security.Claims;
using System.Threading.Tasks;

namespace BPT_Service.Application.CommentService.Command.AddCommentServiceAsync
{
    public class AddCommentServiceAsyncCommand : IAddCommentServiceAsyncCommand
    {
        private readonly IRepository<ServiceComment, Guid> _commentRepository;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IGetOwnServiceInformationQuery _getOwnServiceInformationQuery;
        private readonly UserManager<AppUser> _userManager;

        public AddCommentServiceAsyncCommand(
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

        public async Task<CommandResult<CommentViewModel>> ExecuteAsync(CommentViewModel addComment)
        {
            var userId = _httpContextAccessor.HttpContext.User.Identity.Name;
            var userName = _userManager.FindByIdAsync(userId).Result.UserName;
            try
            {
                if (addComment.ParentId == "")
                {
                    addComment.ParentId = Guid.Empty.ToString();
                }
                var comment = new ServiceComment
                {
                    ContentOfRating = addComment.ContentOfRating,
                    UserId = Guid.Parse(addComment.UserId),
                    ServiceId = Guid.Parse(addComment.ServiceId),
                    ParentId = Guid.Parse(addComment.ParentId)
                };

                await _commentRepository.Add(comment);
                await _commentRepository.SaveAsync();
                var getOwnerService = await _getOwnServiceInformationQuery.ExecuteAsync(addComment.ServiceId);
                await LoggingUser<AddCommentServiceAsyncCommand>.
                    InformationAsync(getOwnerService, userName, addComment.ContentOfRating);
                await Logging<AddCommentServiceAsyncCommand>.InformationAsync(ActionCommand.COMMAND_ADD, userName, JsonConvert.SerializeObject(comment));
                return new CommandResult<CommentViewModel>
                {
                    isValid = true,
                    myModel = new CommentViewModel
                    {
                        ContentOfRating = comment.ContentOfRating,
                        Id = comment.Id.ToString(),
                        ParentId = comment.ParentId.ToString(),
                        ServiceId = comment.ServiceId.ToString(),
                        UserId = comment.UserId.ToString()
                    }
                };
            }
            catch (Exception ex)
            {
                await Logging<AddCommentServiceAsyncCommand>.ErrorAsync(ex, ActionCommand.COMMAND_ADD, userName, "Has error");
                return new CommandResult<CommentViewModel>
                {
                    isValid = false,
                    errorMessage = ex.InnerException.Message.ToString()
                };
            }
        }
    }
}