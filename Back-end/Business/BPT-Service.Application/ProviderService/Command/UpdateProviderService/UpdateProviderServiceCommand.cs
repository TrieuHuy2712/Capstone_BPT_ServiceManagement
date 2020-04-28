using BPT_Service.Application.EmailService.Query.GetAllEmailService;
using BPT_Service.Application.PermissionService.Query.CheckUserIsAdmin;
using BPT_Service.Application.PermissionService.Query.GetPermissionAction;
using BPT_Service.Application.ProviderService.Query.CheckUserIsProvider;
using BPT_Service.Application.ProviderService.ViewModel;
using BPT_Service.Common;
using BPT_Service.Common.Dtos;
using BPT_Service.Common.Helpers;
using BPT_Service.Common.Logging;
using BPT_Service.Model.Entities;
using BPT_Service.Model.Entities.ServiceModel;
using BPT_Service.Model.Enums;
using BPT_Service.Model.Infrastructure.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System;
using System.Security.Claims;
using System.Threading.Tasks;

namespace BPT_Service.Application.ProviderService.Command.UpdateProviderService
{
    public class UpdateProviderServiceCommand : IUpdateProviderServiceCommand
    {
        private readonly ICheckUserIsAdminQuery _checkUserIsAdminQuery;
        private readonly ICheckUserIsProviderQuery _checkUserIsProviderQuery;
        private readonly IGetPermissionActionQuery _getPermissionActionQuery;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IRepository<Provider, Guid> _providerRepository;
        private readonly UserManager<AppUser> _userRepository;

        public UpdateProviderServiceCommand(
            ICheckUserIsAdminQuery checkUserIsAdminQuery,
            ICheckUserIsProviderQuery checkUserIsProviderQuery,
            IGetPermissionActionQuery getPermissionActionQuery,
            IHttpContextAccessor httpContextAccessor,
            IRepository<Provider, Guid> providerRepository,
            UserManager<AppUser> userRepository)
        {
            _checkUserIsAdminQuery = checkUserIsAdminQuery;
            _checkUserIsProviderQuery = checkUserIsProviderQuery;
            _getPermissionActionQuery = getPermissionActionQuery;
            _httpContextAccessor = httpContextAccessor;
            _providerRepository = providerRepository;
            _userRepository = userRepository;
        }

        public async Task<CommandResult<ProviderServiceViewModel>> ExecuteAsync(ProviderServiceViewModel vm)
        {
            var userId = _httpContextAccessor.HttpContext.User.Identity.Name;
            var userName = _userRepository.FindByIdAsync(userId).Result.UserName;
            try
            {
                var checkIsProvider = await _checkUserIsProviderQuery.ExecuteAsync(userId);
                var getProvider = await _providerRepository.FindByIdAsync(Guid.Parse(vm.Id));
                if (getProvider != null)
                {
                    if (getProvider.UserId == Guid.Parse(userId) ||
                        await _checkUserIsAdminQuery.ExecuteAsync(userId) ||
                        await _getPermissionActionQuery.ExecuteAsync(userId, "PROVIDER", ActionSetting.CanUpdate))
                    {
                        var mapping = await MappingProvider(getProvider, vm, userId);
                        _providerRepository.Update(mapping);
                        await _providerRepository.SaveAsync();
                        await Logging<UpdateProviderServiceCommand>.
                           InformationAsync(ActionCommand.COMMAND_UPDATE, userName, JsonConvert.SerializeObject(vm));
                        return new CommandResult<ProviderServiceViewModel>
                        {
                            isValid = true,
                            myModel = vm
                        };
                    }
                    else
                    {
                        await Logging<UpdateProviderServiceCommand>.
                           WarningAsync(ActionCommand.COMMAND_UPDATE, userName, ErrorMessageConstant.ERROR_UPDATE_PERMISSION);
                        return new CommandResult<ProviderServiceViewModel>
                        {
                            isValid = false,
                            errorMessage = ErrorMessageConstant.ERROR_UPDATE_PERMISSION
                        };
                    }
                }
                else
                {
                    await Logging<UpdateProviderServiceCommand>.
                            WarningAsync(ActionCommand.COMMAND_UPDATE, userName, ErrorMessageConstant.ERROR_CANNOT_FIND_ID);
                    return new CommandResult<ProviderServiceViewModel>
                    {
                        isValid = false,
                        errorMessage = ErrorMessageConstant.ERROR_CANNOT_FIND_ID
                    };
                }
            }
            catch (System.Exception ex)
            {
                await Logging<UpdateProviderServiceCommand>.
                        ErrorAsync(ex, ActionCommand.COMMAND_UPDATE, userName, "Has error");
                return new CommandResult<ProviderServiceViewModel>
                {
                    isValid = false,
                    errorMessage = ex.InnerException.ToString()
                };
            }
        }

        private async Task<Provider> MappingProvider(Provider pro, ProviderServiceViewModel vm, string currentUserContext)
        {
            pro.PhoneNumber = vm.PhoneNumber;
            pro.Status = (await _checkUserIsAdminQuery.ExecuteAsync(currentUserContext) ||
                            await _getPermissionActionQuery.ExecuteAsync(currentUserContext, "PROVIDER", ActionSetting.CanUpdate)) ?
                            Status.Active : Status.Pending;
            pro.CityId = vm.CityId;
            pro.TaxCode = vm.TaxCode;
            pro.Description = vm.Description;
            pro.DateModified = DateTime.Now;
            pro.ProviderName = vm.ProviderName;
            pro.Address = vm.Address;
            pro.AvartarPath = vm.AvatarPath;
            return pro;
        }
    }
}