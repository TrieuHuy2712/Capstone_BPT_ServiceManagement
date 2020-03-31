using BPT_Service.Application.FunctionService.ViewModel;
using BPT_Service.Application.PermissionService.Query.CheckUserIsAdmin;
using BPT_Service.Application.PermissionService.Query.GetPermissionAction;
using BPT_Service.Common;
using BPT_Service.Common.Helpers;
using BPT_Service.Model.Entities;
using BPT_Service.Model.Infrastructure.Interfaces;
using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BPT_Service.Application.FunctionService.Command.UpdateParentId
{
    public class UpdateParentIdServiceCommand : IUpdateParentIdServiceCommand
    {
        private readonly IRepository<Function, string> _functionRepository;
        private readonly ICheckUserIsAdminQuery _checkUserIsAdminQuery;
        private readonly IGetPermissionActionQuery _getPermissionActionQuery;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public UpdateParentIdServiceCommand(
            IRepository<Function, string> functionRepository,
            ICheckUserIsAdminQuery checkUserIsAdminQuery,
            IGetPermissionActionQuery getPermissionActionQuery,
            IHttpContextAccessor httpContextAccessor)
        {
            _functionRepository = functionRepository;
            _checkUserIsAdminQuery = checkUserIsAdminQuery;
            _getPermissionActionQuery = getPermissionActionQuery;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<CommandResult<FunctionViewModelinFunctionService>> ExecuteAsync(string sourceId, string targetId, Dictionary<string, int> items)
        {
            try
            {
                //Check user has permission first
                var userId = _httpContextAccessor.HttpContext.User.Identity.Name;
                if (await _checkUserIsAdminQuery.ExecuteAsync(userId) || await _getPermissionActionQuery.ExecuteAsync(userId, "FUNCTION", ActionSetting.CanUpdate))
                {
                    //Update parent id for source
                    var category = await _functionRepository.FindByIdAsync(sourceId);
                    if (category != null)
                    {
                        category.ParentId = targetId;
                        _functionRepository.Update(category);

                        //Get all sibling
                        var sibling = await _functionRepository.FindAllAsync(x => items.ContainsKey(x.Id));
                        foreach (var child in sibling)
                        {
                            child.SortOrder = items[child.Id];
                            _functionRepository.Update(child);
                        }
                        await _functionRepository.SaveAsync();
                        return new CommandResult<FunctionViewModelinFunctionService>
                        {
                            isValid = true,
                            myModel = new FunctionViewModelinFunctionService
                            {
                                IconCss = category.IconCss,
                                Id = category.Id,
                                Name = category.Name,
                                ParentId = category.ParentId,
                                SortOrder = category.SortOrder,
                                Status = category.Status,
                                URL = category.URL
                            }
                        };
                    }
                    return new CommandResult<FunctionViewModelinFunctionService>
                    {
                        isValid = false,
                        errorMessage = ErrorMessageConstant.ERROR_CANNOT_FIND_ID
                    };
                }
                else
                {
                    return new CommandResult<FunctionViewModelinFunctionService>
                    {
                        isValid = false,
                        errorMessage = ErrorMessageConstant.ERROR_UPDATE_PERMISSION
                    };
                }
            }
            catch (System.Exception ex)
            {
                return new CommandResult<FunctionViewModelinFunctionService>
                {
                    isValid = false,
                    errorMessage = ex.InnerException.ToString()
                };
            }
        }
    }
}