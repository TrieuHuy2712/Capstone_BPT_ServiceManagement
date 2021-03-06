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
using System;
using System.Threading.Tasks;

namespace BPT_Service.Application.TagService.Command.UpdateTagServiceAsync
{
    public class UpdateTagServiceAsyncCommand : IUpdateTagServiceAsyncCommand
    {
        private readonly IRepository<Tag, Guid> _tagRepository;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ICheckUserIsAdminQuery _checkUserIsAdminQuery;
        private readonly IGetPermissionActionQuery _getPermissionActionQuery;
        private readonly UserManager<AppUser> _userManager;

        public UpdateTagServiceAsyncCommand(
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

        public async Task<CommandResult<TagViewModel>> ExecuteAsync(TagViewModel userVm)
        {
            var userId = _httpContextAccessor.HttpContext.User.Identity.Name;
            var userName = _userManager.FindByIdAsync(userId).Result.UserName;
            try
            {
                if (await _checkUserIsAdminQuery.ExecuteAsync(userId) ||
                    await _getPermissionActionQuery.ExecuteAsync(userId, ConstantFunctions.TAG, ActionSetting.CanUpdate))
                {
                    var TagUpdate = await _tagRepository.FindByIdAsync(Guid.Parse(userVm.Id));
                    if (TagUpdate != null)
                    {
                        Tag tag = new Tag();
                        tag.TagName = userVm.TagName;
                        _tagRepository.Update(tag);
                        await _tagRepository.SaveAsync();
                        await Logging<UpdateTagServiceAsyncCommand>.
                            InformationAsync(ActionCommand.COMMAND_UPDATE, userName, JsonConvert.SerializeObject(tag));
                        return new CommandResult<TagViewModel>
                        {
                            isValid = true,
                            myModel = new TagViewModel
                            {
                                Id = tag.Id.ToString(),
                                TagName = tag.TagName
                            }
                        };
                    }
                    else
                    {
                        await Logging<UpdateTagServiceAsyncCommand>.
                           WarningAsync(ActionCommand.COMMAND_UPDATE, userName, ErrorMessageConstant.ERROR_CANNOT_FIND_ID);
                        return new CommandResult<TagViewModel>
                        {
                            isValid = false,
                            errorMessage = ErrorMessageConstant.ERROR_CANNOT_FIND_ID
                        };
                    }
                }
                else
                {
                    await Logging<UpdateTagServiceAsyncCommand>.
                           WarningAsync(ActionCommand.COMMAND_UPDATE, userName, ErrorMessageConstant.ERROR_UPDATE_PERMISSION);
                    return new CommandResult<TagViewModel>
                    {
                        isValid = false,
                        errorMessage = ErrorMessageConstant.ERROR_UPDATE_PERMISSION
                    };
                }
            }
            catch (System.Exception ex)
            {
                await Logging<UpdateTagServiceAsyncCommand>.ErrorAsync(ex, ActionCommand.COMMAND_UPDATE, userName, "Has error");
                return new CommandResult<TagViewModel>
                {
                    isValid = false,
                    errorMessage = ex.InnerException.ToString()
                };
            }
        }
    }
}