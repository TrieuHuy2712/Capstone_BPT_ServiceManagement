using BPT_Service.Application.FunctionService.ViewModel;
using BPT_Service.Application.PermissionService.Query.CheckUserIsAdmin;
using BPT_Service.Application.PermissionService.Query.GetPermissionAction;
using BPT_Service.Common;
using BPT_Service.Common.Helpers;
using BPT_Service.Common.Logging;
using BPT_Service.Model.Entities;
using BPT_Service.Model.Infrastructure.Interfaces;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using System.Security.Claims;
using System.Threading.Tasks;

namespace BPT_Service.Application.FunctionService.Command.AddFunctionService
{
    public class AddFunctionServiceCommand : IAddFunctionServiceCommand
    {
        private readonly IRepository<Function, string> _functionRepository;
        private readonly ICheckUserIsAdminQuery _checkUserIsAdminQuery;
        private readonly IGetPermissionActionQuery _getPermissionActionQuery;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public AddFunctionServiceCommand(
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
            var userName = _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value;
            try
            {
                //Check user has permission first
                var userId = _httpContextAccessor.HttpContext.User.Identity.Name;
                if (await _checkUserIsAdminQuery.ExecuteAsync(userId) || await _getPermissionActionQuery.ExecuteAsync(userId, "FUNCTION", ActionSetting.CanCreate))
                {
                    var mappingFunction = MappingFunction(function);
                    await _functionRepository.Add(mappingFunction);
                    await _functionRepository.SaveAsync();
                    await Logging<AddFunctionServiceCommand>.InformationAsync(ActionCommand.COMMAND_ADD, userName, JsonConvert.SerializeObject(mappingFunction));
                    return new CommandResult<FunctionViewModelinFunctionService>
                    {
                        isValid = true,
                        myModel = new FunctionViewModelinFunctionService
                        {
                            IconCss = mappingFunction.IconCss,
                            Id = mappingFunction.Id,
                            Name = mappingFunction.Name,
                            ParentId = mappingFunction.ParentId,
                            SortOrder = mappingFunction.SortOrder,
                            Status = mappingFunction.Status,
                            URL = mappingFunction.URL
                        }
                    };
                }
                else
                {
                    await Logging<AddFunctionServiceCommand>
                        .WarningAsync(ActionCommand.COMMAND_ADD, userName, ErrorMessageConstant.ERROR_ADD_PERMISSION);
                    return new CommandResult<FunctionViewModelinFunctionService>
                    {
                        isValid = false,
                        errorMessage = ErrorMessageConstant.ERROR_ADD_PERMISSION
                    };
                }
            }
            catch (System.Exception ex)
            {
                await Logging<AddFunctionServiceCommand>.ErrorAsync(ex, ActionCommand.COMMAND_ADD, userName, "Has error");
                return new CommandResult<FunctionViewModelinFunctionService>
                {
                    isValid = false,
                    errorMessage = ex.InnerException.ToString()
                };
            }
        }

        public Function MappingFunction(FunctionViewModelinFunctionService function)
        {
            Function newfunction = new Function();
            newfunction.Id = function.Id;
            newfunction.IconCss = function.IconCss;
            newfunction.Name = function.Name;
            newfunction.ParentId = function.ParentId;
            newfunction.SortOrder = function.SortOrder;
            newfunction.Status = function.Status;
            newfunction.URL = function.URL;
            return newfunction;
        }
    }
}