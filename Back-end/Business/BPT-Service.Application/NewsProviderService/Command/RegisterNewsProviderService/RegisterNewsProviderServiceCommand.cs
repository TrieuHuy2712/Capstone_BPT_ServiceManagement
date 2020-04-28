using BPT_Service.Application.NewsProviderService.ViewModel;
using BPT_Service.Application.PermissionService.Query.CheckUserIsAdmin;
using BPT_Service.Application.PermissionService.Query.GetPermissionAction;
using BPT_Service.Application.ProviderService.Query.CheckUserIsProvider;
using BPT_Service.Common;
using BPT_Service.Common.Helpers;
using BPT_Service.Common.Logging;
using BPT_Service.Model.Entities;
using BPT_Service.Model.Entities.ServiceModel.ProviderServiceModel;
using BPT_Service.Model.Enums;
using BPT_Service.Model.Infrastructure.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Newtonsoft.Json;
using System;
using System.Security.Claims;
using System.Threading.Tasks;

namespace BPT_Service.Application.NewsProviderService.Command.RegisterNewsProviderService
{
    public class RegisterNewsProviderServiceCommand : IRegisterNewsProviderServiceCommand
    {
        private readonly IRepository<ProviderNew, int> _newProviderRepository;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ICheckUserIsAdminQuery _checkUserIsAdminQuery;
        private readonly IGetPermissionActionQuery _getPermissionActionQuery;
        private readonly ICheckUserIsProviderQuery _checkUserIsProviderQuery;
        private readonly UserManager<AppUser> _userManager;

        public RegisterNewsProviderServiceCommand(IHttpContextAccessor httpContextAccessor,
        IRepository<ProviderNew, int> newProviderRepository,
        ICheckUserIsAdminQuery checkUserIsAdminQuery,
        IGetPermissionActionQuery getPermissionActionQuery,
        ICheckUserIsProviderQuery checkUserIsProviderQuery,
        UserManager<AppUser> userManager)
        {
            _httpContextAccessor = httpContextAccessor;
            _newProviderRepository = newProviderRepository;
            _checkUserIsAdminQuery = checkUserIsAdminQuery;
            _getPermissionActionQuery = getPermissionActionQuery;
            _checkUserIsProviderQuery = checkUserIsProviderQuery;
            _userManager = userManager;
        }

        public async Task<CommandResult<NewsProviderViewModel>> ExecuteAsync(NewsProviderViewModel vm)
        {
            var userId = _httpContextAccessor.HttpContext.User.Identity.Name;
            var userName = _userManager.FindByIdAsync(userId).Result.UserName;
            try
            {
                var getIsProvider = await _checkUserIsProviderQuery.ExecuteAsync(userId);
                if (await _checkUserIsAdminQuery.ExecuteAsync(userId) ||
                   await _getPermissionActionQuery.ExecuteAsync(userId, "NEWS", ActionSetting.CanCreate) ||
                   getIsProvider.isValid)
                {
                    var mappingProvider = await MappingProvider(vm, Guid.Parse(vm.ProviderId), userId);
                    await _newProviderRepository.Add(mappingProvider);
                    await _newProviderRepository.SaveAsync();
                    vm.Id = mappingProvider.Id;
                    vm.Status = mappingProvider.Status;
                    await Logging<RegisterNewsProviderServiceCommand>.
                        InformationAsync(ActionCommand.COMMAND_ADD, userName, JsonConvert.SerializeObject(vm));
                    return new CommandResult<NewsProviderViewModel>
                    {
                        isValid = true,
                        myModel = vm
                    };
                }
                else
                {
                    await Logging<RegisterNewsProviderServiceCommand>.
                           WarningAsync(ActionCommand.COMMAND_ADD, userName, ErrorMessageConstant.ERROR_DELETE_PERMISSION);
                    return new CommandResult<NewsProviderViewModel>
                    {
                        isValid = true,
                        errorMessage = ErrorMessageConstant.ERROR_ADD_PERMISSION
                    };
                }
            }
            catch (System.Exception ex)
            {
                await Logging<RegisterNewsProviderServiceCommand>.
                       ErrorAsync(ex, ActionCommand.COMMAND_ADD, userName, "Has error");
                return new CommandResult<NewsProviderViewModel>
                {
                    isValid = false,
                    myModel = vm,
                    errorMessage = ex.InnerException.ToString()
                };
            }
        }

        private async Task<ProviderNew> MappingProvider(NewsProviderViewModel vm, Guid providerId, string currentUserContext)
        {
            ProviderNew pro = new ProviderNew();
            pro.Author = vm.Author;
            pro.Status = (await _getPermissionActionQuery.ExecuteAsync(currentUserContext, "PROVIDER", ActionSetting.CanCreate)
                || await _checkUserIsAdminQuery.ExecuteAsync(currentUserContext)) ? Status.Active : Status.Pending;
            pro.Author = vm.Author;
            pro.ProviderId = providerId;
            pro.Title = vm.Title;
            pro.Content = vm.Content;
            pro.DateCreated = DateTime.Now;
            pro.ImgPath = vm.ImgPath;
            return pro;
        }
    }
}