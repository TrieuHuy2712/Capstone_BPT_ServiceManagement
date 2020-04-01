using System;
using System.Threading.Tasks;
using BPT_Service.Application.PermissionService.Query.CheckUserIsAdmin;
using BPT_Service.Application.PermissionService.Query.GetPermissionAction;
using BPT_Service.Application.TagService.ViewModel;
using BPT_Service.Common;
using BPT_Service.Common.Helpers;
using BPT_Service.Model.Entities;
using BPT_Service.Model.Infrastructure.Interfaces;
using Microsoft.AspNetCore.Http;

namespace BPT_Service.Application.TagService.Command.UpdateTagServiceAsync
{
    public class UpdateTagServiceAsyncCommand : IUpdateTagServiceAsyncCommand
    {
        private readonly IRepository<Tag, Guid> _tagRepository;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ICheckUserIsAdminQuery _checkUserIsAdminQuery;
        private readonly IGetPermissionActionQuery _getPermissionActionQuery;

        public UpdateTagServiceAsyncCommand(
            IRepository<Tag, Guid> tagRepository, 
            IHttpContextAccessor httpContextAccessor, 
            ICheckUserIsAdminQuery checkUserIsAdminQuery, 
            IGetPermissionActionQuery getPermissionActionQuery)
        {
            _tagRepository = tagRepository;
            _httpContextAccessor = httpContextAccessor;
            _checkUserIsAdminQuery = checkUserIsAdminQuery;
            _getPermissionActionQuery = getPermissionActionQuery;
        }

        public async Task<CommandResult<TagViewModel>> ExecuteAsync(TagViewModel userVm)
        {
            try
            {
                var userId = _httpContextAccessor.HttpContext.User.Identity.Name;
                if (await _checkUserIsAdminQuery.ExecuteAsync(userId) ||
                    await _getPermissionActionQuery.ExecuteAsync(userId, "TAG", ActionSetting.CanUpdate))
                {
                    var TagUpdate = await _tagRepository.FindByIdAsync(Guid.Parse(userVm.Id));
                    if (TagUpdate != null)
                    {
                        Tag tag = new Tag();
                        tag.Id = Guid.Parse(userVm.Id);
                        tag.TagName = userVm.TagName;
                        _tagRepository.Update(tag);
                        await _tagRepository.SaveAsync();
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
                        return new CommandResult<TagViewModel>
                        {
                            isValid = false,
                            errorMessage = ErrorMessageConstant.ERROR_CANNOT_FIND_ID
                        };
                    }
                }
                else
                {
                    return new CommandResult<TagViewModel>
                    {
                        isValid = false,
                        errorMessage = ErrorMessageConstant.ERROR_UPDATE_PERMISSION
                    };
                }
            }
            catch (System.Exception ex)
            {
                return new CommandResult<TagViewModel>
                {
                    isValid = false,
                    errorMessage = ex.InnerException.ToString()
                };
            }
        }
    }
}