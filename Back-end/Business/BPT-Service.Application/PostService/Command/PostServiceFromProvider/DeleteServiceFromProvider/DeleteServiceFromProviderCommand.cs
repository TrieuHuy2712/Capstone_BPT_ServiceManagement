using BPT_Service.Application.PermissionService.Query.CheckUserIsAdmin;
using BPT_Service.Application.PermissionService.Query.GetPermissionAction;
using BPT_Service.Application.PostService.ViewModel;
using BPT_Service.Application.ProviderService.Query.CheckUserIsProvider;
using BPT_Service.Common;
using BPT_Service.Common.Constants;
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

namespace BPT_Service.Application.PostService.Command.PostServiceFromProvider.DeleteServiceFromProvider
{
    public class DeleteServiceFromProviderCommand : IDeleteServiceFromProviderCommand
    {
        private readonly ICheckUserIsAdminQuery _checkUserIsAdminQuery;
        private readonly ICheckUserIsProviderQuery _checkUserIsProvider;
        private readonly IGetPermissionActionQuery _getPermissionActionQuery;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IRepository<Model.Entities.ServiceModel.ProviderServiceModel.ProviderService, int> _providerServiceRepository;
        private readonly IRepository<Provider, Guid> _providerRepository;
        private readonly IRepository<Service, Guid> _postServiceRepository;
        private readonly UserManager<AppUser> _userManager;

        public DeleteServiceFromProviderCommand(
            ICheckUserIsAdminQuery checkUserIsAdminQuery,
            ICheckUserIsProviderQuery checkUserIsProvider,
            IGetPermissionActionQuery getPermissionActionQuery,
            IHttpContextAccessor httpContextAccessor,
            IRepository<Model.Entities.ServiceModel.ProviderServiceModel.ProviderService, int> providerServiceRepository,
            IRepository<Provider, Guid> providerRepository,
            IRepository<Service, Guid> postServiceRepository,
            UserManager<AppUser> userManager)
        {
            _checkUserIsAdminQuery = checkUserIsAdminQuery;
            _checkUserIsProvider = checkUserIsProvider;
            _getPermissionActionQuery = getPermissionActionQuery;
            _httpContextAccessor = httpContextAccessor;
            _providerServiceRepository = providerServiceRepository;
            _providerRepository = providerRepository;
            _postServiceRepository = postServiceRepository;
            _userManager = userManager;
        }

        public async Task<CommandResult<PostServiceViewModel>> ExecuteAsync(string idService)
        {
            var userId = _httpContextAccessor.HttpContext.User.Identity.Name;
            var userName = _userManager.FindByIdAsync(userId).Result.UserName;
            try
            {
                //Get Id Service and Check it has permission by checkUserIsProvider
                var findIdService = await _postServiceRepository.FindByIdAsync(Guid.Parse(idService));
                var checkUserIsProvider = await _checkUserIsProvider.ExecuteAsync(userId);
                if (findIdService != null)
                {
                    var findProviderService = await _providerServiceRepository.FindSingleAsync(x => x.ServiceId == findIdService.Id);
                    //Check permission can delete
                    if (findProviderService != null ||
                        await _getPermissionActionQuery.ExecuteAsync(userId, ConstantFunctions.SERVICE, ActionSetting.CanDelete) ||
                        await _checkUserIsAdminQuery.ExecuteAsync(userId) ||
                        findProviderService.ProviderId == Guid.Parse(checkUserIsProvider.myModel.Id.ToString()))
                    {
                        _providerServiceRepository.Remove(findProviderService);
                        _postServiceRepository.Remove(findIdService);
                        await _postServiceRepository.SaveAsync();
                        await Logging<DeleteServiceFromProviderCommand>.
                            InformationAsync(ActionCommand.COMMAND_DELETE,userName,findIdService.ServiceName+" has been removed");
                        return new CommandResult<PostServiceViewModel>
                        {
                            isValid = true
                        };
                    }
                    else
                    {
                        await Logging<DeleteServiceFromProviderCommand>.
                            WarningAsync(ActionCommand.COMMAND_DELETE, userName, ErrorMessageConstant.ERROR_DELETE_PERMISSION);
                        return new CommandResult<PostServiceViewModel>
                        {
                            isValid = false,
                            errorMessage = ErrorMessageConstant.ERROR_DELETE_PERMISSION
                        };
                    }
                }
                else
                {
                    await Logging<DeleteServiceFromProviderCommand>.
                            WarningAsync(ActionCommand.COMMAND_DELETE, userName, ErrorMessageConstant.ERROR_CANNOT_FIND_ID);
                    return new CommandResult<PostServiceViewModel>
                    {
                        isValid = false,
                        errorMessage = ErrorMessageConstant.ERROR_CANNOT_FIND_ID
                    };
                }
            }
            catch (System.Exception ex)
            {
                await Logging<DeleteServiceFromProviderCommand>.
                        ErrorAsync(ex, ActionCommand.COMMAND_DELETE, userName, "Had error");
                return new CommandResult<PostServiceViewModel>
                {
                    isValid = false,
                    errorMessage = ex.InnerException.ToString()
                };
            }
        }
    }
}