using BPT_Service.Application.PermissionService.Query.CheckUserIsAdmin;
using BPT_Service.Application.PermissionService.Query.GetPermissionAction;
using BPT_Service.Application.PostService.ViewModel;
using BPT_Service.Application.ProviderService.Query.CheckUserIsProvider;
using BPT_Service.Common;
using BPT_Service.Common.Helpers;
using BPT_Service.Common.Logging;
using BPT_Service.Model.Entities;
using BPT_Service.Model.Entities.ServiceModel;
using BPT_Service.Model.Infrastructure.Interfaces;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using System;
using System.Security.Claims;
using System.Threading.Tasks;

namespace BPT_Service.Application.PostService.Command.PostServiceFromUser.DeleteServiceFromUser
{
    public class DeleteServiceFromUserCommand : IDeleteServiceFromUserCommand
    {
        private readonly ICheckUserIsAdminQuery _checkUserIsAdminQuery;
        private readonly ICheckUserIsProviderQuery _checkUserIsProvider;
        private readonly IGetPermissionActionQuery _getPermissionActionQuery;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IRepository<Model.Entities.ServiceModel.UserServiceModel.UserService, int> _userServiceRepository;
        private readonly IRepository<Service, Guid> _postServiceRepository;

        public DeleteServiceFromUserCommand(ICheckUserIsAdminQuery checkUserIsAdminQuery,
            ICheckUserIsProviderQuery checkUserIsProvider,
            IGetPermissionActionQuery getPermissionActionQuery,
            IHttpContextAccessor httpContextAccessor,
            IRepository<Model.Entities.ServiceModel.UserServiceModel.UserService, int> userServiceRepository,
            IRepository<Service, Guid> postServiceRepository)
        {
            _checkUserIsAdminQuery = checkUserIsAdminQuery;
            _checkUserIsProvider = checkUserIsProvider;
            _getPermissionActionQuery = getPermissionActionQuery;
            _httpContextAccessor = httpContextAccessor;
            _userServiceRepository = userServiceRepository;
            _postServiceRepository = postServiceRepository;
        }

        public async Task<CommandResult<PostServiceViewModel>> ExecuteAsync(string idService)
        {
            var userName = _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value;
            try
            {
                var findIdService = await _postServiceRepository.FindByIdAsync(Guid.Parse(idService));
                var userId = _httpContextAccessor.HttpContext.User.Identity.Name;
                var checkUserIsProvider = await _checkUserIsProvider.ExecuteAsync();
                if (findIdService != null)
                {
                    var findUserService = await _userServiceRepository.FindSingleAsync(x => x.ServiceId == findIdService.Id);
                    //Check permission can delete
                    if (findUserService != null ||
                        await _getPermissionActionQuery.ExecuteAsync(userId, "SERVICE", ActionSetting.CanDelete) ||
                        await _checkUserIsAdminQuery.ExecuteAsync(userId) ||
                        findUserService.UserId == Guid.Parse(userId))
                    {
                        _userServiceRepository.Remove(findUserService);
                        _postServiceRepository.Remove(findIdService);
                        await _postServiceRepository.SaveAsync();
                        await Logging<DeleteServiceFromUserCommand>.
                            InformationAsync(ActionCommand.COMMAND_DELETE, userName, JsonConvert.SerializeObject(findUserService));
                        return new CommandResult<PostServiceViewModel>
                        {
                            isValid = true
                        };
                    }
                    else
                    {
                        await Logging<DeleteServiceFromUserCommand>.
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
                    await Logging<DeleteServiceFromUserCommand>.
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
                await Logging<DeleteServiceFromUserCommand>.
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