using BPT_Service.Application.PermissionService.Query.CheckUserIsAdmin;
using BPT_Service.Application.PermissionService.Query.GetPermissionAction;
using BPT_Service.Application.PostService.ViewModel;
using BPT_Service.Application.ProviderService.Query.CheckUserIsProvider;
using BPT_Service.Common;
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
        private readonly UserManager<AppUser> _userManager;
        private readonly IRepository<Model.Entities.ServiceModel.TagService, int> _serviceOfTagRepository;
        private readonly IRepository<ServiceImage, int> _serviceImageRepository;

        public UpdatePostServiceCommand(
            ICheckUserIsAdminQuery checkUserIsAdminQuery,
            ICheckUserIsProviderQuery checkUserIsProvider,
            IGetPermissionActionQuery getPermissionActionQuery,
            IHttpContextAccessor httpContext, IRepository<Provider, Guid> providerRepository,
            IRepository<Service, Guid> serviceRepository,
            IRepository<Tag, Guid> tagServiceRepository,
            UserManager<AppUser> userManager,
            IRepository<Model.Entities.ServiceModel.TagService, int> serviceOfTagRepository,
            IRepository<ServiceImage, int> serviceImageRepository)
        {
            _checkUserIsAdminQuery = checkUserIsAdminQuery;
            _checkUserIsProvider = checkUserIsProvider;
            _getPermissionActionQuery = getPermissionActionQuery;
            _httpContext = httpContext;
            _providerRepository = providerRepository;
            _serviceRepository = serviceRepository;
            _tagServiceRepository = tagServiceRepository;
            _userManager = userManager;
            _serviceOfTagRepository = serviceOfTagRepository;
            _serviceImageRepository = serviceImageRepository;
        }

        public async Task<CommandResult<PostServiceViewModel>> ExecuteAsync(PostServiceViewModel vm)
        {
            var userId = _httpContext.HttpContext.User.Identity.Name;
            var userName = _userManager.FindByIdAsync(userId).Result.UserName;
            try
            {
                var getPermissionForService = await IsOwnService(Guid.Parse(vm.Id));

                if (getPermissionForService.isValid || await _checkUserIsAdminQuery.ExecuteAsync(userId)
                    || await _getPermissionActionQuery.ExecuteAsync(userId, "SERVICE", ActionSetting.CanUpdate))
                {
                    var currentService = getPermissionForService.myModel;
                    var service = await _serviceRepository.FindByIdAsync(Guid.Parse(vm.Id));
                    if (service == null)
                    {
                        return new CommandResult<PostServiceViewModel>
                        {
                            isValid = false,
                            errorMessage = ErrorMessageConstant.ERROR_CANNOT_FIND_ID
                        };
                    }
                    List<Tag> newTag = new List<Tag>();
                    var tagDelete = await RemoveTag(Guid.Parse(vm.Id), vm.tagofServices);
                    foreach (var item in vm.tagofServices)
                    {
                        if (item.isAdd == true)
                        {
                            newTag.Add(new Tag
                            {
                                TagName = item.TagName
                            });
                        }
                    }
                    var deletImage = await RemoveImage(Guid.Parse(vm.Id), vm.listImages);
                    _serviceImageRepository.RemoveMultiple(deletImage);
                    await _tagServiceRepository.Add(newTag);
                    _tagServiceRepository.RemoveMultiple(tagDelete);
                    var mappingService = await MappingService(vm, service, userId);
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
                    await Logging<UpdatePostServiceCommand>.
                        InformationAsync(ActionCommand.COMMAND_UPDATE, userName, JsonConvert.SerializeObject(vm));
                    return new CommandResult<PostServiceViewModel>
                    {
                        isValid = true,
                        myModel = vm,
                    };
                }
                await Logging<UpdatePostServiceCommand>.
                        WarningAsync(ActionCommand.COMMAND_UPDATE, userName, ErrorMessageConstant.ERROR_UPDATE_PERMISSION);
                return new CommandResult<PostServiceViewModel>
                {
                    isValid = false,
                    errorMessage = ErrorMessageConstant.ERROR_UPDATE_PERMISSION
                };
            }
            catch (System.Exception ex)
            {
                await Logging<UpdatePostServiceCommand>.
                        ErrorAsync(ex, ActionCommand.COMMAND_UPDATE, userName, "Has error");
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

        private async Task<Service> MappingService(PostServiceViewModel vm, Service sv, string currentUserContext)
        {
            sv.CategoryId = vm.CategoryId;
            sv.DateModified = DateTime.Now;
            sv.PriceOfService = vm.PriceOfService;
            sv.Description = vm.Description;
            sv.ServiceName = vm.ServiceName;
            sv.Status = (await _getPermissionActionQuery.ExecuteAsync(currentUserContext, "SERVICE", ActionSetting.CanCreate)
                || await _checkUserIsAdminQuery.ExecuteAsync(currentUserContext)) ? Status.Active : Status.Pending;
            sv.ServiceImages = vm.listImages.Select(x => new ServiceImage
            {
                Path = x.Path != null ? x.Path : "",
                DateCreated = DateTime.Now,
            }).ToList();
            sv.TagServices = vm.tagofServices.Where(t => t.isDelete == false && t.isAdd == false).Select(x => new Model.Entities.ServiceModel.TagService
            {
                TagId = Guid.Parse(x.TagId),
            }).ToList();
            return sv;
        }

        public async Task<List<Tag>> RemoveTag(Guid id, List<TagofServiceViewModel> listTag)
        {
            var userTag = await _serviceOfTagRepository.FindAllAsync(x => x.ServiceId == id);
            var tag = await _tagServiceRepository.FindAllAsync();
            var joinTag = (from uT in userTag.ToList()
                           join t in tag.ToList()
                           on uT.TagId equals t.Id
                           select new TagofServiceViewModel
                           {
                               TagId = t.Id.ToString(),
                               TagName = t.TagName,
                           });

            foreach (var item in joinTag)
            {
                int count = 0;
                foreach (var item2 in listTag)
                {
                    if (item.TagName == item2.TagName)
                    {
                        count++;
                    }
                }
                if (count != 0)
                {
                    item.isDelete = true;
                }
            }
            List<Tag> returnTag = new List<Tag>();
            foreach (var item in joinTag)
            {
                if (item.isDelete)
                {
                    var findTag = await _tagServiceRepository.FindSingleAsync(x => x.Id == Guid.Parse(item.TagId));
                    returnTag.Add(findTag);
                }
            }
            return returnTag;
        }

        public async Task<List<ServiceImage>> RemoveImage(Guid id, List<PostServiceImageViewModel> listImages)
        {
            var findAllImageWithId = await _serviceImageRepository.FindAllAsync(x => x.ServiceId == id);
            List<ServiceImage> serviceImages = new List<ServiceImage>();
            foreach (var item in findAllImageWithId)
            {
                int count = 0;
                foreach (var item2 in listImages)
                {
                    if (item.Path == item2.Path)
                    {
                        count++;
                    }
                }
                if (count == 0)
                {
                    serviceImages.Add(item);
                }
            }
            return serviceImages;
        }
    }
}