using BPT_Service.Application.FunctionService.ViewModel;
using BPT_Service.Application.PermissionService.Query.CheckUserIsAdmin;
using BPT_Service.Application.PermissionService.Query.GetPermissionAction;
using BPT_Service.Common;
using BPT_Service.Common.Constants;
using BPT_Service.Common.Helpers;
using BPT_Service.Common.Logging;
using BPT_Service.Model.Entities;
using BPT_Service.Model.Infrastructure.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Newtonsoft.Json;
using System.Security.Claims;
using System.Threading.Tasks;

namespace BPT_Service.Application.FunctionService.Command.UpdateFunctionService
{
    public class UpdateFunctionServiceCommand : IUpdateFunctionServiceCommand
    {
        private readonly IRepository<Function, string> _functionRepository;
        private readonly ICheckUserIsAdminQuery _checkUserIsAdminQuery;
        private readonly IGetPermissionActionQuery _getPermissionActionQuery;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly UserManager<AppUser> _userManager;

        public UpdateFunctionServiceCommand(
            IRepository<Function, string> functionRepository,
            ICheckUserIsAdminQuery checkUserIsAdminQuery,
            IGetPermissionActionQuery getPermissionActionQuery,
            IHttpContextAccessor httpContextAccessor,
            UserManager<AppUser> userManager)
        {
            _functionRepository = functionRepository;
            _checkUserIsAdminQuery = checkUserIsAdminQuery;
            _getPermissionActionQuery = getPermissionActionQuery;
            _httpContextAccessor = httpContextAccessor;
            _userManager = userManager;
        }

        public async Task<CommandResult<FunctionViewModelinFunctionService>> ExecuteAsync(FunctionViewModelinFunctionService function)
        {
            var userId = _httpContextAccessor.HttpContext.User.Identity.Name;
            var userName = _userManager.FindByIdAsync(userId).Result.UserName;
            try
            {
                //Check user has permission first
                if (await _checkUserIsAdminQuery.ExecuteAsync(userId) || await _getPermissionActionQuery.ExecuteAsync(userId, ConstantFunctions.FUNCTION, ActionSetting.CanUpdate))
                {
                    var functionDb = await _functionRepository.FindByIdAsync(function.Id);

                    if (functionDb != null)
                    {
                        functionDb.IconCss = function.IconCss;
                        functionDb.Id = function.Id;
                        functionDb.Name = function.Name;
                        functionDb.ParentId = function.ParentId;
                        functionDb.Status = function.Status;
                        functionDb.URL = function.URL;
                        _functionRepository.Update(functionDb);
                        await _functionRepository.SaveAsync();
                        await Logging<UpdateFunctionServiceCommand>.
                            InformationAsync(ActionCommand.COMMAND_UPDATE, userName, JsonConvert.SerializeObject(functionDb));
                        return new CommandResult<FunctionViewModelinFunctionService>
                        {
                            isValid = true,
                            myModel = function,
                        };
                    }
                    await Logging<UpdateFunctionServiceCommand>
                        .WarningAsync(ActionCommand.COMMAND_UPDATE, userName, ErrorMessageConstant.ERROR_CANNOT_FIND_ID);
                    return new CommandResult<FunctionViewModelinFunctionService>
                    {
                        isValid = false,
                        errorMessage = ErrorMessageConstant.ERROR_CANNOT_FIND_ID
                    };
                }
                else
                {
                    await Logging<UpdateFunctionServiceCommand>
                        .WarningAsync(ActionCommand.COMMAND_UPDATE, userName, ErrorMessageConstant.ERROR_UPDATE_PERMISSION);
                    return new CommandResult<FunctionViewModelinFunctionService>
                    {
                        isValid = false,
                        errorMessage = ErrorMessageConstant.ERROR_UPDATE_PERMISSION
                    };
                }
            }
            catch (System.Exception ex)
            {
                await Logging<UpdateFunctionServiceCommand>.ErrorAsync(ex, ActionCommand.COMMAND_UPDATE, userName, ErrorMessageConstant.ERROR_UPDATE_PERMISSION);
                return new CommandResult<FunctionViewModelinFunctionService>
                {
                    isValid = false,
                    errorMessage = ex.InnerException.ToString()
                };
            }
        }
    }
}