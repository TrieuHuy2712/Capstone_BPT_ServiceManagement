using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BPT_Service.Application.PostService.ViewModel;
using BPT_Service.Model.Entities;
using BPT_Service.Model.Entities.ServiceModel;
using BPT_Service.Model.Enums;
using BPT_Service.Model.Infrastructure.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;

namespace BPT_Service.Application.PostService.Command.UpdatePostService
{
    public class UpdatePostServiceCommand : IUpdatePostServiceCommand
    {

        private readonly IRepository<Service, Guid> _serviceRepository;
        private readonly IRepository<Provider, Guid> _providerRepository;
        private readonly UserManager<AppUser> _userManager;
        private readonly IRepository<Tag, Guid> _tagServiceRepository;

        private readonly IHttpContextAccessor _httpContext;
        public UpdatePostServiceCommand(
            IRepository<Service, Guid> serviceRepository,
            UserManager<AppUser> userManager,
        IRepository<Provider, Guid> providerRepository,
        IHttpContextAccessor httpContext,
        IRepository<Tag, Guid> tagServiceRepository
        )
        {
            _serviceRepository = serviceRepository;
            _providerRepository = providerRepository;
            _userManager = userManager;
            _httpContext = httpContext;
            _tagServiceRepository = tagServiceRepository;
        }
        public async Task<CommandResult<PostServiceViewModel>> ExecuteAsync(PostServiceViewModel vm)
        {
            try
            {
                var getPermissionForService = await IsOwnService(vm.Id);
                if (getPermissionForService.isValid)
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
                                Id = item.TagId,
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
                            mappingTag.TagId = tag.TagId;
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
                    errorMessage = "Cannot update",
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
            sv.Status = Status.Pending;
            sv.ServiceImages = vm.listImages.Select(x => new ServiceImage
            {
                Path = x.Path,
                DateCreated = DateTime.Now,
                ServiceId = x.ServiceId
            }).ToList();
            sv.TagServices = vm.tagofServices.Select(x => new Model.Entities.ServiceModel.TagService
            {
                ServiceId = x.ServiceId,
                TagId = x.TagId,
            }).ToList();
            return sv;
        }
    }
}