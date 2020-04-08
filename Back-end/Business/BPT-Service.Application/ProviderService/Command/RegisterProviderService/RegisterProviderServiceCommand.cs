using BPT_Service.Application.PermissionService.Query.CheckUserIsAdmin;
using BPT_Service.Application.PermissionService.Query.GetPermissionAction;
using BPT_Service.Application.ProviderService.Query.CheckUserIsProvider;
using BPT_Service.Application.ProviderService.ViewModel;
using BPT_Service.Common.Helpers;
using BPT_Service.Common.Logging;
using BPT_Service.Model.Entities;
using BPT_Service.Model.Entities.ServiceModel;
using BPT_Service.Model.Enums;
using BPT_Service.Model.Infrastructure.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Newtonsoft.Json;
using System;
using System.Threading.Tasks;

namespace BPT_Service.Application.ProviderService.Command.RegisterProviderService
{
    public class RegisterProviderServiceCommand : IRegisterProviderServiceCommand
    {
        private readonly IRepository<Provider, Guid> _providerRepository;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ICheckUserIsAdminQuery _checkUserIsAdminQuery;
        private readonly IGetPermissionActionQuery _getPermissionActionQuery;
        private readonly ICheckUserIsProviderQuery _checkUserIsProviderQuery;
        private readonly UserManager<AppUser> _userManager;

        public RegisterProviderServiceCommand(
            IRepository<Provider, Guid> providerRepository,
            IHttpContextAccessor httpContextAccessor,
            ICheckUserIsAdminQuery checkUserIsAdminQuery,
            IGetPermissionActionQuery getPermissionActionQuery,
            ICheckUserIsProviderQuery checkUserIsProviderQuery,
            UserManager<AppUser> userManager)
        {
            _providerRepository = providerRepository;
            _httpContextAccessor = httpContextAccessor;
            _checkUserIsAdminQuery = checkUserIsAdminQuery;
            _getPermissionActionQuery = getPermissionActionQuery;
            _checkUserIsProviderQuery = checkUserIsProviderQuery;
            _userManager = userManager;
        }

        public async Task<CommandResult<ProviderServiceViewModel>> ExecuteAsync(ProviderServiceViewModel vm)
        {
            var userId = _httpContextAccessor.HttpContext.User.Identity.Name;
            var userName = _userManager.FindByIdAsync(userId).Result.UserName;
            try
            {
                var checkUserIsProvider = await _checkUserIsProviderQuery.ExecuteAsync();
                var mappingProvider = await MappingProvider(vm, Guid.Parse(userId), userId);
                if((await _getPermissionActionQuery.ExecuteAsync(userId, "PROVIDER", ActionSetting.CanCreate)
                || await _checkUserIsAdminQuery.ExecuteAsync(userId)))
                {
                    var findUserId = await _userManager.FindByIdAsync(vm.UserId);
                    await _userManager.AddToRoleAsync(findUserId, "Provider");
                }
                await _providerRepository.Add(mappingProvider);
                await _providerRepository.SaveAsync();
                vm.Id = mappingProvider.Id.ToString();
                await Logging<RegisterProviderServiceCommand>.
                       InformationAsync(ActionCommand.COMMAND_ADD, userName, JsonConvert.SerializeObject(mappingProvider));
                return new CommandResult<ProviderServiceViewModel>
                {
                    isValid = true,
                    myModel = vm
                };
            }
            catch (System.Exception ex)
            {
                await Logging<RegisterProviderServiceCommand>.
                       ErrorAsync(ex, ActionCommand.COMMAND_ADD, userName, "Has error");
                return new CommandResult<ProviderServiceViewModel>
                {
                    isValid = false,
                    myModel = vm,
                    errorMessage = ex.InnerException.ToString()
                };
            }
        }

        private async Task<Provider> MappingProvider(ProviderServiceViewModel vm, Guid userId, string currentUserContext)
        {
            Provider pro = new Provider();
            pro.PhoneNumber = vm.PhoneNumber;
            pro.Status = (await _getPermissionActionQuery.ExecuteAsync(currentUserContext, "PROVIDER", ActionSetting.CanCreate)
                || await _checkUserIsAdminQuery.ExecuteAsync(currentUserContext)) ? Status.Active : Status.Pending;
            pro.CityId = vm.CityId;
            pro.UserId = vm.UserId==null?userId:Guid.Parse(vm.UserId);
            pro.TaxCode = vm.TaxCode;
            pro.Description = vm.Description;
            pro.DateCreated = DateTime.Now;
            pro.ProviderName = vm.ProviderName;
            pro.Address = vm.Address;
            pro.AvartarPath = vm.AvatarPath;
            return pro;
        }
    }
}