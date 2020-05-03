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

namespace BPT_Service.Application.CommentService.Command.UpdateCommentServiceAsync
{
    public class UpdateCommentServiceAsyncCommand : IUpdateCommentServiceAsyncCommand
    {
        private readonly IRepository<ServiceComment, int> _commentRepository;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IGetOwnServiceInformationQuery _getOwnServiceInformationQuery;
        private readonly UserManager<AppUser> _userManager;

        public UpdateCommentServiceAsyncCommand(
            IRepository<ServiceComment, int> commentRepository,
            IHttpContextAccessor httpContextAccessor,
            IGetOwnServiceInformationQuery getOwnServiceInformationQuery,
            UserManager<AppUser> userManager)
        {
            _commentRepository = commentRepository;
            _httpContextAccessor = httpContextAccessor;
            _getOwnServiceInformationQuery = getOwnServiceInformationQuery;
            _userManager = userManager;
        }

        public async Task<CommandResult<CommentViewModel>> ExecuteAsync(CommentViewModel commentserviceVm)
        {
            var userId = _httpContextAccessor.HttpContext.User.Identity.Name;
            var userName = _userManager.FindByIdAsync(userId).Result.UserName;
            try
            {
                var CommentUpdate = await _commentRepository.FindByIdAsync(commentserviceVm.Id);
                if (CommentUpdate != null)
                {
                    CommentUpdate.ContentOfRating = commentserviceVm.ContentOfRating;
                    CommentUpdate.UserId = Guid.Parse(commentserviceVm.UserId);
                    CommentUpdate.ServiceId = Guid.Parse(commentserviceVm.ServiceId);
                    var getUserService = await _getOwnServiceInformationQuery.ExecuteAsync(CommentUpdate.ServiceId.ToString());
                    _commentRepository.Update(CommentUpdate);
                    await _commentRepository.SaveAsync();
                    await LoggingUser<UpdateCommentServiceAsyncCommand>.InformationAsync(getUserService, userName, CommentUpdate.ContentOfRating);
                    await Logging<UpdateCommentServiceAsyncCommand>.InformationAsync(ActionCommand.COMMAND_UPDATE, userName, userName + " updated " + CommentUpdate.ContentOfRating);
                    return new CommandResult<CommentViewModel>
                    {
                        isValid = true,
                        myModel = new CommentViewModel
                        {
                            Id = CommentUpdate.Id,
                            UserId = CommentUpdate.UserId.ToString(),
                            ServiceId = CommentUpdate.ServiceId.ToString(),
                            ContentOfRating = CommentUpdate.ContentOfRating
                        }
                    };
                }
                else
                {
                    await Logging<UpdateCommentServiceAsyncCommand>.ErrorAsync(ActionCommand.COMMAND_UPDATE, userName, ErrorMessageConstant.ERROR_CANNOT_FIND_ID);
                    return new CommandResult<CommentViewModel>
                    {
                        isValid = false,
                        errorMessage = ErrorMessageConstant.ERROR_CANNOT_FIND_ID
                    };
                }
            }
            catch (System.Exception ex)
            {
                await Logging<UpdateCommentServiceAsyncCommand>.ErrorAsync(ex, ActionCommand.COMMAND_UPDATE, userName, "Has error");
                return new CommandResult<CommentViewModel>
                {
                    isValid = false,
                    errorMessage = ex.InnerException.ToString()
                };
            }
        }
    }
}