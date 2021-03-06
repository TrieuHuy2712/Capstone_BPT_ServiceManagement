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
using System.Security.Claims;
using System.Threading.Tasks;

namespace BPT_Service.Application.TagService.Command.AddServiceAsync
{
    public class AddTagServiceAsyncCommand : IAddTagServiceAsyncCommand
    {
        private readonly IRepository<Tag, Guid> _tagRepository;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ICheckUserIsAdminQuery _checkUserIsAdminQuery;
        private readonly IGetPermissionActionQuery _getPermissionActionQuery;
        private readonly UserManager<AppUser> _userManager;

        public AddTagServiceAsyncCommand(
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
                    await _getPermissionActionQuery.ExecuteAsync(userId, ConstantFunctions.TAG, ActionSetting.CanCreate))
                {
                    Tag tag = new Tag();
                    tag.TagName = userVm.TagName;
                    await _tagRepository.Add(tag);
                    await _tagRepository.SaveAsync();
                    await Logging<AddTagServiceAsyncCommand>.InformationAsync(ActionCommand.COMMAND_ADD, userName, JsonConvert.SerializeObject(tag));
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
                    await Logging<AddTagServiceAsyncCommand>.
                        WarningAsync(ActionCommand.COMMAND_ADD, userName, ErrorMessageConstant.ERROR_ADD_PERMISSION);
                    return new CommandResult<TagViewModel>
                    {
                        isValid = false,
                        errorMessage = ErrorMessageConstant.ERROR_ADD_PERMISSION
                    };
                }
            }
            catch (System.Exception ex)
            {
                await Logging<AddTagServiceAsyncCommand>.ErrorAsync(ex, ActionCommand.COMMAND_ADD, userName, "Has error");
                return new CommandResult<TagViewModel>
                {
                    isValid = false,
                    errorMessage = ex.InnerException.ToString()
                };
            }
        }
    }
}