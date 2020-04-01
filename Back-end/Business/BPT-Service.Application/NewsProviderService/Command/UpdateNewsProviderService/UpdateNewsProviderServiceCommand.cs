using BPT_Service.Application.NewsProviderService.ViewModel;
using BPT_Service.Application.PermissionService.Query.CheckUserIsAdmin;
using BPT_Service.Application.PermissionService.Query.GetPermissionAction;
using BPT_Service.Application.ProviderService.Query.CheckUserIsProvider;
using BPT_Service.Common;
using BPT_Service.Common.Helpers;
using BPT_Service.Model.Entities;
using BPT_Service.Model.Entities.ServiceModel.ProviderServiceModel;
using BPT_Service.Model.Infrastructure.Interfaces;
using Microsoft.AspNetCore.Http;
using System;
using System.Threading.Tasks;

namespace BPT_Service.Application.NewsProviderService.Command.UpdateNewsProviderService
{
    public class UpdateNewsProviderServiceCommand : IUpdateNewsProviderServiceCommand
    {
        private readonly IRepository<ProviderNew, int> _providerNewsRepository;
        private readonly IHttpContextAccessor _httpContext;
        private readonly ICheckUserIsAdminQuery _checkUserIsAdminQuery;
        private readonly IGetPermissionActionQuery _getPermissionActionQuery;
        private readonly ICheckUserIsProviderQuery _checkUserIsProviderQuery;

        public UpdateNewsProviderServiceCommand(
            IRepository<ProviderNew, int> providerNewsRepository,
            IHttpContextAccessor httpContext,
            ICheckUserIsAdminQuery checkUserIsAdminQuery,
            IGetPermissionActionQuery getPermissionActionQuery,
            ICheckUserIsProviderQuery checkUserIsProviderQuery)
        {
            _providerNewsRepository = providerNewsRepository;
            _httpContext = httpContext;
            _checkUserIsAdminQuery = checkUserIsAdminQuery;
            _getPermissionActionQuery = getPermissionActionQuery;
            _checkUserIsProviderQuery = checkUserIsProviderQuery;
        }

        public async Task<CommandResult<NewsProviderViewModel>> ExecuteAsync(NewsProviderViewModel vm)
        {
            try
            {
                var findProviderNew = await _providerNewsRepository.FindByIdAsync(vm.Id);
                if (findProviderNew != null)
                {
                    var userId = _httpContext.HttpContext.User.Identity.Name;
                    var getIsProvider = await _checkUserIsProviderQuery.ExecuteAsync();
                    if (await _checkUserIsAdminQuery.ExecuteAsync(userId) ||
                       await _getPermissionActionQuery.ExecuteAsync(userId, "NEWS", ActionSetting.CanUpdate) ||
                       (getIsProvider.isValid && getIsProvider.myModel.Id == findProviderNew.ProviderId.ToString()))
                    {
                        var mappingNewsProvider = MappingProvider(findProviderNew, vm);
                        _providerNewsRepository.Update(mappingNewsProvider);
                        await _providerNewsRepository.SaveAsync();
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
                            isValid = false,
                            errorMessage = ErrorMessageConstant.ERROR_UPDATE_PERMISSION
                        };
                    }
                }
                else
                {
                    return new CommandResult<NewsProviderViewModel>
                    {
                        isValid = false,
                        errorMessage = ErrorMessageConstant.ERROR_CANNOT_FIND_ID
                    };
                }
            }
            catch (System.Exception ex)
            {
                return new CommandResult<NewsProviderViewModel>
                {
                    isValid = false,
                    errorMessage = ex.InnerException.ToString()
                };
            }
        }

        private ProviderNew MappingProvider(ProviderNew pro, NewsProviderViewModel vm)
        {
            pro.Author = vm.Author;
            pro.Status = vm.Status;
            pro.Author = vm.Author;
            pro.ProviderId = Guid.Parse(vm.ProviderId);
            pro.Title = vm.Title;
            pro.Content = vm.Content;
            pro.DateModified = DateTime.Now;
            pro.ImgPath = vm.ImgPath;
            return pro;
        }
    }
}