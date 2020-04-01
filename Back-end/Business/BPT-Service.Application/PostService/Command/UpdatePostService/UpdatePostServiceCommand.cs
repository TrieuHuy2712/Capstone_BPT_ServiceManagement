using BPT_Service.Application.PermissionService.Query.CheckUserIsAdmin;
using BPT_Service.Application.PermissionService.Query.GetPermissionAction;
using BPT_Service.Application.PostService.ViewModel;
using BPT_Service.Application.ProviderService.Query.CheckUserIsProvider;
using BPT_Service.Common;
using BPT_Service.Common.Helpers;
using BPT_Service.Model.Entities;
using BPT_Service.Model.Entities.ServiceModel;
using BPT_Service.Model.Enums;
using BPT_Service.Model.Infrastructure.Interfaces;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BPT_Service.Application.PostService.Command.UpdatePostService
{
    public class UpdatePostServiceCommand : IUpdatePostServiceCommand
    {
        private readonly ICheckUserIsAdminQuery _checkUserIsAdminQuery;
        private readonly ICheckUserIsProviderQuery _checkUserIsProvider;
        private readonly IGetPermissionActionQuery _getPermissionActionQuery;
        private readonly IHttpContextAccessor _httpContext;
        private readonly IRepository<Provider, Guid> _providerRepository;
        private readonly IRepository<Service, Guid> _serviceRepository;
        private readonly IRepository<Tag, Guid> _tagServiceRepository;

        public UpdatePostServiceCommand(
            ICheckUserIsAdminQuery checkUserIsAdminQuery, 
            ICheckUserIsProviderQuery checkUserIsProvider, 
            IGetPermissionActionQuery getPermissionActionQuery, 
            IHttpContextAccessor httpContext, IRepository<Provider, Guid> providerRepository, 
            IRepository<Service, Guid> serviceRepository, 
            IRepository<Tag, Guid> tagServiceRepository)
        {
            _checkUserIsAdminQuery = checkUserIsAdminQuery;
            _checkUserIsProvider = checkUserIsProvider;
            _getPermissionActionQuery = getPermissionActionQuery;
            _httpContext = httpContext;
            _providerRepository = providerRepository;
            _serviceRepository = serviceRepository;
            _tagServiceRepository = tagServiceRepository;
        }

        public async Task<CommandResult<PostServiceViewModel>> ExecuteAsync(PostServiceViewModel vm)
        {
            try
            {
                var getPermissionForService = await IsOwnService(Guid.Parse(vm.Id));
                var userId = _httpContext.HttpContext.User.Identity.Name;

                if (getPermissionForService.isValid || await _checkUserIsAdminQuery.ExecuteAsync(userId)
                    || await _getPermissionActionQuery.ExecuteAsync(userId, "SERVICE", ActionSetting.CanUpdate))
                {
                    var currentService = getPermissionForService.myModel;
                    List<Tag> newTag = new List<Tag>();
                    List<Tag> deleteTag = new List<Tag>();
                    foreach (var item in vm.tagofServices)
                    {
                        if (item.isAdd == true)
                        {
                            newTag.Add(new Tag
                            {
                                TagName = item.TagName
                            });
                        }
                        if (item.isDelete)
                        {
                            deleteTag.Add(new Tag
                            {
                                Id = Guid.Parse(item.TagId),
                                TagName = item.TagName
                            });
                        }
                    }
                    await _tagServiceRepository.Add(newTag);
                    _tagServiceRepository.RemoveMultiple(deleteTag);
                    var mappingService = MappingService(vm, getPermissionForService.myModel);
                    mappingService.TagServices = null;
                    foreach (var tag in vm.tagofServices)
                    {
                        Model.Entities.ServiceModel.TagService mappingTag = new Model.Entities.ServiceModel.TagService();
                        if (tag.isAdd || !tag.isDelete)
                        {
                            mappingTag.TagId = Guid.Parse(tag.TagId);
                            mappingService.TagServices.Add(mappingTag);
                        }
                    }
                    _serviceRepository.Update(mappingService);
                    await _tagServiceRepository.SaveAsync();
                    await _serviceRepository.SaveAsync();
                    return new CommandResult<PostServiceViewModel>
                    {
                        isValid = true,
                        myModel = vm,
                    };
                }
                return new CommandResult<PostServiceViewModel>
                {
                    isValid = false,
                    errorMessage = ErrorMessageConstant.ERROR_UPDATE_PERMISSION
                };
            }
            catch (System.Exception ex)
            {
                return new CommandResult<PostServiceViewModel>
                {
                    isValid = false,
                    errorMessage = ex.InnerException.ToString()
                };
            }
        }

        private async Task<CommandResult<Service>> IsOwnService(Guid idService)
        {
            var getCurrentId = _httpContext.HttpContext.User.Identity.Name;
            var checkOwnProvider = await _providerRepository.FindAllAsync(x => x.UserId == Guid.Parse(getCurrentId));
            var checkOwnService = await _serviceRepository.FindSingleAsync(x => x.UserServices.ServiceId == idService);
            foreach (var item in checkOwnProvider)
            {
                if (item.Id == checkOwnService.ProviderServices.ProviderId)
                {
                    return new CommandResult<Service>
                    {
                        isValid = true,
                        myModel = checkOwnService
                    };
                }
            }

            if (checkOwnService.UserServices.UserId == Guid.Parse(getCurrentId))
            {
                return new CommandResult<Service>
                {
                    isValid = true,
                    myModel = checkOwnService
                };
            }

            if (checkOwnService == null)
            {
                return new CommandResult<Service>
                {
                    isValid = false,
                    errorMessage = "Cannot find service"
                };
            }
            return new CommandResult<Service>
            {
                isValid = false,
                errorMessage = "You don't have permission"
            };
        }

        private Service MappingService(PostServiceViewModel vm, Service sv)
        {
            sv.CategoryId = vm.CategoryId;
            sv.DateModified = DateTime.Now;
            sv.PriceOfService = vm.PriceOfService;
            sv.Description = vm.Description;
            sv.ServiceName = vm.ServiceName;
            sv.Status =  IsOwnService(sv.Id).Result.isValid ? Status.Pending:Status.Active;
            sv.ServiceImages = vm.listImages.Select(x => new ServiceImage
            {
                Path = x.Path,
                DateCreated = DateTime.Now,
                ServiceId = x.ServiceId
            }).ToList();
            sv.TagServices = vm.tagofServices.Select(x => new Model.Entities.ServiceModel.TagService
            {
                ServiceId = Guid.Parse(x.ServiceId),
                TagId = Guid.Parse(x.TagId),
            }).ToList();
            return sv;
        }
    }
}