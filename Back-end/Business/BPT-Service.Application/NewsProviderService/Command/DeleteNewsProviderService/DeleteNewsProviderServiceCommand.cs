using BPT_Service.Application.PermissionService.Query.CheckUserIsAdmin;
using BPT_Service.Application.PermissionService.Query.GetPermissionAction;
using BPT_Service.Application.ProviderService.Query.CheckUserIsProvider;
using BPT_Service.Common;
using BPT_Service.Common.Helpers;
using BPT_Service.Common.Logging;
using BPT_Service.Model.Entities;
using BPT_Service.Model.Entities.ServiceModel.ProviderServiceModel;
using BPT_Service.Model.Infrastructure.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Newtonsoft.Json;
using System.Threading.Tasks;

namespace BPT_Service.Application.NewsProviderService.Command.DeleteNewsProviderService
{
    public class DeleteNewsProviderServiceCommand : IDeleteNewsProviderServiceCommand
    {
        private readonly IRepository<ProviderNew, int> _providerNewRepository;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ICheckUserIsAdminQuery _checkUserIsAdminQuery;
        private readonly IGetPermissionActionQuery _getPermissionActionQuery;
        private readonly ICheckUserIsProviderQuery _checkUserIsProviderQuery;
        private readonly UserManager<AppUser> _userManager;

        public DeleteNewsProviderServiceCommand(
            IRepository<ProviderNew, int> providerNewRepository,
            IHttpContextAccessor httpContextAccessor,
            ICheckUserIsAdminQuery checkUserIsAdminQuery,
            IGetPermissionActionQuery getPermissionActionQuery,
            ICheckUserIsProviderQuery checkUserIsProviderQuery,
            UserManager<AppUser> userManager)
        {
            _providerNewRepository = providerNewRepository;
            _httpContextAccessor = httpContextAccessor;
            _checkUserIsAdminQuery = checkUserIsAdminQuery;
            _getPermissionActionQuery = getPermissionActionQuery;
            _checkUserIsProviderQuery = checkUserIsProviderQuery;
            _userManager = userManager;
        }

        public async Task<CommandResult<ProviderNew>> ExecuteAsync(int id)
        {
            var userId = _httpContextAccessor.HttpContext.User.Identity.Name;
            var userName = _userManager.FindByIdAsync(userId).Result.UserName;
            try
            {
                var getId = await _providerNewRepository.FindByIdAsync(id);
                if (getId != null)
                {
                    var checkUserIsProvider = await _checkUserIsProviderQuery.ExecuteAsync();
                    //Check permission
                    if (await _checkUserIsAdminQuery.ExecuteAsync(userId) ||
                        await _getPermissionActionQuery.ExecuteAsync(userId, "NEWS", ActionSetting.CanDelete) ||
                        (checkUserIsProvider.isValid && checkUserIsProvider.myModel.Id == getId.ProviderId.ToString()))
                    {
                        _providerNewRepository.Remove(getId.Id);
                        await _providerNewRepository.SaveAsync();
                        //Write Log
                        await Logging<DeleteNewsProviderServiceCommand>.
                            InformationAsync(ActionCommand.COMMAND_DELETE, userName, "Has been delete news provider:"+getId.Title+"_"+getId.Author);
                        return new CommandResult<ProviderNew>
                        {
                            isValid = true,
                            myModel = getId
                        };
                    }
                    else
                    {
                        await Logging<DeleteNewsProviderServiceCommand>.
                        WarningAsync(ActionCommand.COMMAND_DELETE, userName, ErrorMessageConstant.ERROR_DELETE_PERMISSION);
                        return new CommandResult<ProviderNew>
                        {
                            isValid = false,
                            errorMessage = ErrorMessageConstant.ERROR_DELETE_PERMISSION
                        };
                    }
                }
                else
                {
                    await Logging<DeleteNewsProviderServiceCommand>.
                        WarningAsync(ActionCommand.COMMAND_DELETE, userName, ErrorMessageConstant.ERROR_CANNOT_FIND_ID);
                    return new CommandResult<ProviderNew>
                    {
                        isValid = false,
                        errorMessage = ErrorMessageConstant.ERROR_CANNOT_FIND_ID
                    };
                }
            }
            catch (System.Exception ex)
            {
                await Logging<DeleteNewsProviderServiceCommand>.
                       ErrorAsync(ex, ActionCommand.COMMAND_APPROVE, userName, "Has error");
                return new CommandResult<ProviderNew>
                {
                    isValid = false,
                    errorMessage = ex.InnerException.ToString()
                };
            }
        }
    }
}