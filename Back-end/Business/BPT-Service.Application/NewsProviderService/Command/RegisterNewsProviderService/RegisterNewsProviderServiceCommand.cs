using BPT_Service.Application.NewsProviderService.ViewModel;
using BPT_Service.Application.PermissionService.Query.CheckUserIsAdmin;
using BPT_Service.Application.PermissionService.Query.GetPermissionAction;
using BPT_Service.Application.ProviderService.Query.CheckUserIsProvider;
using BPT_Service.Common;
using BPT_Service.Common.Helpers;
using BPT_Service.Model.Entities;
using BPT_Service.Model.Entities.ServiceModel;
using BPT_Service.Model.Entities.ServiceModel.ProviderServiceModel;
using BPT_Service.Model.Enums;
using BPT_Service.Model.Infrastructure.Interfaces;
using Microsoft.AspNetCore.Http;
using System;
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

        public RegisterNewsProviderServiceCommand(IHttpContextAccessor httpContextAccessor,
        IRepository<ProviderNew, int> newProviderRepository,
        ICheckUserIsAdminQuery checkUserIsAdminQuery,
        IGetPermissionActionQuery getPermissionActionQuery,
        ICheckUserIsProviderQuery checkUserIsProviderQuery)
        {
            _httpContextAccessor = httpContextAccessor;
            _newProviderRepository = newProviderRepository;
            _checkUserIsAdminQuery = checkUserIsAdminQuery;
            _getPermissionActionQuery = getPermissionActionQuery;
            _checkUserIsProviderQuery = checkUserIsProviderQuery;
        }

        public async Task<CommandResult<NewsProviderViewModel>> ExecuteAsync(NewsProviderViewModel vm)
        {
            try
            {
                var getIsProvider = await _checkUserIsProviderQuery.ExecuteAsync();
                var userId = _httpContextAccessor.HttpContext.User.Identity.Name;
                if (await _checkUserIsAdminQuery.ExecuteAsync(userId) ||
                   await _getPermissionActionQuery.ExecuteAsync(userId, "NEWS", ActionSetting.CanCreate) ||
                   getIsProvider.isValid)
                {
                    var mappingProvider = MappingProvider(vm, Guid.Parse(vm.ProviderId));
                    await _newProviderRepository.Add(mappingProvider);
                    await _newProviderRepository.SaveAsync();
                    vm.Id = mappingProvider.Id;
                    return new CommandResult<NewsProviderViewModel>
                    {
                        isValid = true,
                        myModel = vm
                    };
                }
                else
                {
                    return new CommandResult<NewsProviderViewModel>
                    {
                        isValid = true,
                        errorMessage = ErrorMessageConstant.ERROR_ADD_PERMISSION
                    };
                }
            }
            catch (System.Exception ex)
            {
                return new CommandResult<NewsProviderViewModel>
                {
                    isValid = false,
                    myModel = vm,
                    errorMessage = ex.InnerException.ToString()
                };
            }
        }

        private ProviderNew MappingProvider(NewsProviderViewModel vm, Guid providerId)
        {
            ProviderNew pro = new ProviderNew();
            pro.Author = vm.Author;
            pro.Status = Status.Pending;
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