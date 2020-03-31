using BPT_Service.Application.EmailService.Query.GetAllEmailService;
using BPT_Service.Application.PermissionService.Query.CheckUserIsAdmin;
using BPT_Service.Application.PermissionService.Query.GetPermissionAction;
using BPT_Service.Application.ProviderService.Query.CheckUserIsProvider;
using BPT_Service.Application.ProviderService.ViewModel;
using BPT_Service.Common;
using BPT_Service.Common.Dtos;
using BPT_Service.Common.Helpers;
using BPT_Service.Model.Entities;
using BPT_Service.Model.Entities.ServiceModel;
using BPT_Service.Model.Enums;
using BPT_Service.Model.Infrastructure.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using System;
using System.Threading.Tasks;

namespace BPT_Service.Application.ProviderService.Command.UpdateProviderService
{
    public class UpdateProviderServiceCommand : IUpdateProviderServiceCommand
    {
        private readonly ICheckUserIsAdminQuery _checkUserIsAdminQuery;
        private readonly ICheckUserIsProviderQuery _checkUserIsProviderQuery;
        private readonly IGetAllEmailServiceQuery _getAllEmailServiceQuery;
        private readonly IGetPermissionActionQuery _getPermissionActionQuery;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IOptions<EmailConfigModel> _config;
        private readonly IRepository<Provider, Guid> _providerRepository;
        private readonly UserManager<AppUser> _userRepository;

        public UpdateProviderServiceCommand(
            ICheckUserIsAdminQuery checkUserIsAdminQuery,
            ICheckUserIsProviderQuery checkUserIsProviderQuery,
            IGetAllEmailServiceQuery getAllEmailServiceQuery,
            IGetPermissionActionQuery getPermissionActionQuery,
            IHttpContextAccessor httpContextAccessor,
            IOptions<EmailConfigModel> config,
            IRepository<Provider, Guid> providerRepository,
            UserManager<AppUser> userRepository)
        {
            _checkUserIsAdminQuery = checkUserIsAdminQuery;
            _checkUserIsProviderQuery = checkUserIsProviderQuery;
            _getAllEmailServiceQuery = getAllEmailServiceQuery;
            _getPermissionActionQuery = getPermissionActionQuery;
            _httpContextAccessor = httpContextAccessor;
            _config = config;
            _providerRepository = providerRepository;
            _userRepository = userRepository;
        }

        public async Task<CommandResult<ProviderServiceViewModel>> ExecuteAsync(ProviderServiceViewModel vm)
        {
            try
            {
                var userId = _httpContextAccessor.HttpContext.User.Identity.Name;
                var checkIsProvider = await _checkUserIsProviderQuery.ExecuteAsync();
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
                        return new CommandResult<ProviderServiceViewModel>
                        {
                            isValid = true,
                            myModel = vm
                        };
                    }
                    else
                    {
                        return new CommandResult<ProviderServiceViewModel>
                        {
                            isValid = false,
                            errorMessage = ErrorMessageConstant.ERROR_UPDATE_PERMISSION
                        };
                    }
                }
                else
                {
                    return new CommandResult<ProviderServiceViewModel>
                    {
                        isValid = false,
                        errorMessage = ErrorMessageConstant.ERROR_CANNOT_FIND_ID
                    };
                }
            }
            catch (System.Exception ex)
            {
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
                            Status.Active : Status.UpdatePending;
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