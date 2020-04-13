using BPT_Service.Application.PermissionService.Query.CheckUserIsAdmin;
using BPT_Service.Application.PermissionService.Query.GetPermissionAction;
using BPT_Service.Common;
using BPT_Service.Common.Helpers;
using BPT_Service.Common.Logging;
using BPT_Service.Model.Entities;
using BPT_Service.Model.Entities.ServiceModel;
using BPT_Service.Model.Infrastructure.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Newtonsoft.Json;
using System;
using System.Security.Claims;
using System.Threading.Tasks;

namespace BPT_Service.Application.ProviderService.Command.DeleteProviderService
{
    public class DeleteProviderServiceCommand : IDeleteProviderServiceCommand
    {
        private readonly IRepository<Provider, Guid> _providerRepository;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IGetPermissionActionQuery _getPermissionActionQuery;
        private readonly ICheckUserIsAdminQuery _checkUserIsAdminQuery;
        private readonly UserManager<AppUser> _userManager;

        public DeleteProviderServiceCommand(
            IRepository<Provider, Guid> providerRepository,
            IHttpContextAccessor httpContextAccessor,
            IGetPermissionActionQuery getPermissionActionQuery,
            ICheckUserIsAdminQuery checkUserIsAdminQuery,
            UserManager<AppUser> userManager)
        {
            _providerRepository = providerRepository;
            _httpContextAccessor = httpContextAccessor;
            _getPermissionActionQuery = getPermissionActionQuery;
            _checkUserIsAdminQuery = checkUserIsAdminQuery;
            _userManager = userManager;
        }

        public async Task<CommandResult<Provider>> ExecuteAsync(string id)
        {
            var userId = _httpContextAccessor.HttpContext.User.Identity.Name;
            var userName = _userManager.FindByIdAsync(userId).Result.UserName;
            try
            {
                var newId = Guid.Parse(id);
                if (await _checkUserIsAdminQuery.ExecuteAsync(userId) || await _getPermissionActionQuery.ExecuteAsync(userId, "PROVIDER", ActionSetting.CanDelete))
                {
                    var getId = await _providerRepository.FindByIdAsync(newId);
                    if (getId != null)
                    {
                        //Remove Provider Role
                        _providerRepository.Remove(newId);
                        await _providerRepository.SaveAsync();
                        await Logging<DeleteProviderServiceCommand>.
                            InformationAsync(ActionCommand.COMMAND_DELETE, userName, getId.ProviderName+"has been deleted");
                        return new CommandResult<Provider>
                        {
                            isValid = true,
                            myModel = getId
                        };
                    }
                    else
                    {
                        await Logging<DeleteProviderServiceCommand>.
                           WarningAsync(ActionCommand.COMMAND_DELETE, userName, ErrorMessageConstant.ERROR_CANNOT_FIND_ID);

                        return new CommandResult<Provider>
                        {
                            isValid = false,
                            errorMessage = ErrorMessageConstant.ERROR_CANNOT_FIND_ID
                        };
                    }
                }
                else
                {
                    await Logging<DeleteProviderServiceCommand>.
                           WarningAsync(ActionCommand.COMMAND_DELETE, userName, ErrorMessageConstant.ERROR_DELETE_PERMISSION);

                    return new CommandResult<Provider>
                    {
                        isValid = false,
                        errorMessage = ErrorMessageConstant.ERROR_DELETE_PERMISSION
                    };
                }
            }
            catch (System.Exception ex)
            {
                await Logging<DeleteProviderServiceCommand>.
                       ErrorAsync(ex, ActionCommand.COMMAND_APPROVE, userName, "Has error");
                return new CommandResult<Provider>
                {
                    isValid = false,
                    errorMessage = ex.InnerException.ToString()
                };
            }
        }
    }
}