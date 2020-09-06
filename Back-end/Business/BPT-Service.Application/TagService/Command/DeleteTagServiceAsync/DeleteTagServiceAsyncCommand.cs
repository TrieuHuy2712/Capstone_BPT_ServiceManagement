using System;
using System.Security.Claims;
using System.Threading.Tasks;
using BPT_Service.Application.PermissionService.Query.CheckUserIsAdmin;
using BPT_Service.Application.PermissionService.Query.GetPermissionAction;
using BPT_Service.Application.TagService.ViewModel;
using BPT_Service.Common;
using BPT_Service.Common.Constants;
using BPT_Service.Common.Helpers;
using BPT_Service.Common.Logging;
using BPT_Service.Model.Entities;
using BPT_Service.Model.Infrastructure.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Newtonsoft.Json;

namespace BPT_Service.Application.TagService.Command.DeleteServiceAsync
{
    public class DeleteTagServiceAsyncCommand : IDeleteTagServiceAsyncCommand
    {
        private readonly IRepository<Tag, Guid> _tagRepository;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ICheckUserIsAdminQuery _checkUserIsAdminQuery;
        private readonly IGetPermissionActionQuery _getPermissionActionQuery;
        private readonly UserManager<AppUser> _userManager;

        public DeleteTagServiceAsyncCommand(
            IRepository<Tag, Guid> tagRepository, 
            IHttpContextAccessor httpContextAccessor, 
            ICheckUserIsAdminQuery checkUserIsAdminQuery, 
            IGetPermissionActionQuery getPermissionActionQuery,
            UserManager<AppUser> userManager)
        {
            _tagRepository = tagRepository;
            _httpContextAccessor = httpContextAccessor;
            _checkUserIsAdminQuery = checkUserIsAdminQuery;
            _getPermissionActionQuery = getPermissionActionQuery;
            _userManager = userManager;
        }

        public async Task<CommandResult<TagViewModel>> ExecuteAsync(Guid id)
        {
            var userId = _httpContextAccessor.HttpContext.User.Identity.Name;
            var userName = _userManager.FindByIdAsync(userId).Result.UserName;
            try
            {
                if (await _checkUserIsAdminQuery.ExecuteAsync(userId) ||
                    await _getPermissionActionQuery.ExecuteAsync(userId, ConstantFunctions.TAG, ActionSetting.CanDelete))
                {
                    var TagDel = await _tagRepository.FindByIdAsync(id);
                    if (TagDel != null)
                    {
                        _tagRepository.Remove(TagDel);
                        await _tagRepository.SaveAsync();
                        await Logging<DeleteTagServiceAsyncCommand>.
                            InformationAsync(ActionCommand.COMMAND_DELETE, userName, JsonConvert.SerializeObject(TagDel));
                        return new CommandResult<TagViewModel>
                        {
                            isValid = true,
                            myModel = new TagViewModel
                            {
                                Id = TagDel.Id.ToString(),
                                TagName = TagDel.TagName
                            }
                        };
                    }
                    else
                    {
                        await Logging<DeleteTagServiceAsyncCommand>.
                        WarningAsync(ActionCommand.COMMAND_DELETE, userName, ErrorMessageConstant.ERROR_CANNOT_FIND_ID);
                        return new CommandResult<TagViewModel>
                        {
                            isValid = false,
                            errorMessage = ErrorMessageConstant.ERROR_CANNOT_FIND_ID
                        };
                    }
                }
                else
                {
                    await Logging<DeleteTagServiceAsyncCommand>.
                        WarningAsync(ActionCommand.COMMAND_DELETE, userName, ErrorMessageConstant.ERROR_DELETE_PERMISSION);
                    return new CommandResult<TagViewModel>
                    {
                        isValid = false,
                        errorMessage = ErrorMessageConstant.ERROR_DELETE_PERMISSION
                    };
                }
            }
            catch (System.Exception ex)
            {
                await Logging<DeleteTagServiceAsyncCommand>.ErrorAsync(ex, ActionCommand.COMMAND_DELETE, userName, "Has error");
                return new CommandResult<TagViewModel>
                {
                    isValid = true,
                    errorMessage = ex.InnerException.ToString()
                };
            }
        }
    }
}