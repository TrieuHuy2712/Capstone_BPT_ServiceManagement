using BPT_Service.Application.FunctionService.ViewModel;
using BPT_Service.Application.PermissionService.Query.CheckUserIsAdmin;
using BPT_Service.Application.PermissionService.Query.GetPermissionAction;
using BPT_Service.Common;
using BPT_Service.Common.Helpers;
using BPT_Service.Model.Entities;
using BPT_Service.Model.Infrastructure.Interfaces;
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;

namespace BPT_Service.Application.FunctionService.Command.UpdateFunctionService
{
    public class UpdateFunctionServiceCommand : IUpdateFunctionServiceCommand
    {
        private readonly IRepository<Function, string> _functionRepository;
        private readonly ICheckUserIsAdminQuery _checkUserIsAdminQuery;
        private readonly IGetPermissionActionQuery _getPermissionActionQuery;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public UpdateFunctionServiceCommand(
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

        public async Task<CommandResult<FunctionViewModelinFunctionService>> ExecuteAsync(FunctionViewModelinFunctionService function)
        {
            try
            {
                //Check user has permission first
                var userId = _httpContextAccessor.HttpContext.User.Identity.Name;
                if (await _checkUserIsAdminQuery.ExecuteAsync(userId) || await _getPermissionActionQuery.ExecuteAsync(userId, "FUNCTION", ActionSetting.CanUpdate))
                {
                    var functionDb = await _functionRepository.FindByIdAsync(function.Id);

                    if (functionDb != null)
                    {
                        functionDb.IconCss = function.IconCss;
                        functionDb.Id = function.Id;
                        functionDb.Name = function.Name;
                        functionDb.ParentId = function.ParentId;
                        functionDb.SortOrder = function.SortOrder;
                        functionDb.Status = function.Status;
                        functionDb.URL = function.URL;
                        _functionRepository.Update(functionDb);
                        await _functionRepository.SaveAsync();
                        return new CommandResult<FunctionViewModelinFunctionService>
                        {
                            isValid = true,
                            myModel = function,
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