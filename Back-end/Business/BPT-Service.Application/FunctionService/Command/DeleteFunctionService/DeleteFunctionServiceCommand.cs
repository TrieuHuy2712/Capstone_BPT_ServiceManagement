using BPT_Service.Application.FunctionService.ViewModel;
using BPT_Service.Application.PermissionService.Query.CheckUserIsAdmin;
using BPT_Service.Application.PermissionService.Query.GetPermissionAction;
using BPT_Service.Common;
using BPT_Service.Common.Helpers;
using BPT_Service.Common.Logging;
using BPT_Service.Model.Entities;
using BPT_Service.Model.Infrastructure.Interfaces;
using Microsoft.AspNetCore.Http;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace BPT_Service.Application.FunctionService.Command.DeleteFunctionService
{
    public class DeleteFunctionServiceCommand : IDeleteFunctionServiceCommand
    {
        private readonly IRepository<Function, string> _functionRepository;
        private readonly ICheckUserIsAdminQuery _checkUserIsAdminQuery;
        private readonly IGetPermissionActionQuery _getPermissionActionQuery;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public DeleteFunctionServiceCommand(
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

        public async Task<CommandResult<FunctionViewModelinFunctionService>> ExecuteAsync(string id)
        {
            var userName = _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value;
            try
            {
                //Check user has permission first
                var userId = _httpContextAccessor.HttpContext.User.Identity.Name;
                if (await _checkUserIsAdminQuery.ExecuteAsync(userId) || await _getPermissionActionQuery.ExecuteAsync(userId, "FUNCTION", ActionSetting.CanDelete))
                {
                    var getChildItem = await _functionRepository.FindAllAsync(x => x.ParentId == id);
                    if (getChildItem.Count() > 0)
                    {
                        foreach (var item in getChildItem)
                        {
                            item.ParentId = null;
                            _functionRepository.Update(item);
                        }
                    }
                    _functionRepository.Remove(id);
                    await _functionRepository.SaveAsync();
                    await Logging<DeleteFunctionServiceCommand>.InformationAsync(ActionCommand.COMMAND_DELETE, userName, "Delete " + id);
                    return new CommandResult<FunctionViewModelinFunctionService>
                    {
                        isValid = true,
                        myModel = null
                    };
                }
                else
                {
                    await Logging<DeleteFunctionServiceCommand>
                        .WarningAsync(ActionCommand.COMMAND_DELETE, userName, ErrorMessageConstant.ERROR_DELETE_PERMISSION);
                    return new CommandResult<FunctionViewModelinFunctionService>
                    {
                        isValid = false,
                        errorMessage = ErrorMessageConstant.ERROR_DELETE_PERMISSION
                    };
                }
            }
            catch (System.Exception ex)
            {
                await Logging<DeleteFunctionServiceCommand>.ErrorAsync(ex, ActionCommand.COMMAND_DELETE, userName, "Has error");
                return new CommandResult<FunctionViewModelinFunctionService>
                {
                    isValid = false,
                    errorMessage = ex.InnerException.ToString()
                };
            }
        }
    }
}