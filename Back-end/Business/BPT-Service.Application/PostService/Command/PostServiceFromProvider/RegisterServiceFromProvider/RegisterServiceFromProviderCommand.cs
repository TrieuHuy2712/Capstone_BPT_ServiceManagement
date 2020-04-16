using BPT_Service.Application.PermissionService.Query.CheckUserIsAdmin;
using BPT_Service.Application.PermissionService.Query.GetPermissionAction;
using BPT_Service.Application.PostService.ViewModel;
using BPT_Service.Application.ProviderService.Query.CheckUserIsProvider;
using BPT_Service.Application.ProviderService.Query.GetByIdProviderService;
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
using System.Security.Claims;
using System.Threading.Tasks;

namespace BPT_Service.Application.PostService.Command.PostServiceFromProvider.RegisterServiceFromProvider
{
    public class RegisterServiceFromProviderCommand : IRegisterServiceFromProviderCommand
    {
        private readonly IRepository<Service, Guid> _postServiceRepository;
        private readonly IRepository<Tag, Guid> _tagServiceRepository;
        private readonly IRepository<Model.Entities.ServiceModel.ProviderServiceModel.ProviderService, int> _providerServiceRepository;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ICheckUserIsProviderQuery _checkUserIsProvider;
        private readonly ICheckUserIsAdminQuery _checkUserIsAdminQuery;
        private readonly IGetPermissionActionQuery _getPermissionActionQuery;
        private readonly UserManager<AppUser> _userManager;
        private readonly IRepository<Provider, Guid> _providerRepository;

        public RegisterServiceFromProviderCommand(IRepository<Service, Guid> postServiceRepository
        , IRepository<Model.Entities.ServiceModel.ProviderServiceModel.ProviderService, int> providerServiceRepository,
        IRepository<Tag, Guid> tagServiceRepository,
        IHttpContextAccessor httpContextAccessor,
        ICheckUserIsProviderQuery checkUserIsProvider,
        ICheckUserIsAdminQuery checkUserIsAdminQuery,
        IGetPermissionActionQuery getPermissionActionQuery,
        UserManager<AppUser> userManager,
        IRepository<Provider, Guid> providerRepository)
        {
            _postServiceRepository = postServiceRepository;
            _providerServiceRepository = providerServiceRepository;
            _httpContextAccessor = httpContextAccessor;
            _tagServiceRepository = tagServiceRepository;
            _checkUserIsProvider = checkUserIsProvider;
            _checkUserIsAdminQuery = checkUserIsAdminQuery;
            _getPermissionActionQuery = getPermissionActionQuery;
            _userManager = userManager;
            _providerRepository = providerRepository;
        }

        public async Task<CommandResult<PostServiceViewModel>> ExecuteAsync(PostServiceViewModel vm)
        {
            var userId = _httpContextAccessor.HttpContext.User.Identity.Name;
            var userName = _userManager.FindByIdAsync(userId).Result.UserName;
            try
            {
                var checkUserIsAdmin = await _checkUserIsProvider.ExecuteAsync();
                if (await _getPermissionActionQuery.ExecuteAsync(userId, "SERVICE", ActionSetting.CanCreate) ||
                    await _checkUserIsAdminQuery.ExecuteAsync(userId) || checkUserIsAdmin.isValid)
                {
                    //Add new tag when isAdd equal true
                    List<Tag> newTag = new List<Tag>();
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
                    await _tagServiceRepository.Add(newTag);

                    //Mapping between ViewModel and Model of Service
                    var mappingService = MappingService(vm);
                    await _postServiceRepository.Add(mappingService);

                    //Mapping between ViewModel and Model of ServiceProvider
                    var mappingProviderService = MappingProviderService(mappingService.Id, Guid.Parse(vm.ProviderId));
                    await _providerServiceRepository.Add(mappingProviderService);

                    //Add new Tag with Id in TagService
                    foreach (var item in newTag)
                    {
                        Model.Entities.ServiceModel.TagService mappingTag = new Model.Entities.ServiceModel.TagService();
                        mappingTag.TagId = item.Id;
                        mappingService.TagServices.Add(mappingTag);
                    }

                    await _tagServiceRepository.SaveAsync();
                    vm.Id = mappingService.Id.ToString();
                    //Write Log
                    await Logging<RegisterServiceFromProviderCommand>.
                        InformationAsync(ActionCommand.COMMAND_ADD,userName,JsonConvert.SerializeObject(vm));
                    return new CommandResult<PostServiceViewModel>
                    {
                        isValid = true,
                        myModel = new PostServiceViewModel
                        {
                            Id = mappingService.Id.ToString(),
                            DateCreated = mappingService.DateCreated,
                            IsProvider = true,
                            Author = _providerRepository.FindSingleAsync(x=>x.Id == Guid.Parse(vm.ProviderId)).Result.ProviderName,
                            ProviderId = vm.ProviderId,
                            AvtService = mappingService.ServiceImages.Where(x=>x.isAvatar==true).FirstOrDefault().Path,
                            listImages = mappingService.ServiceImages.Select(x=>new PostServiceImageViewModel {
                                ImageId = x.Id,
                                IsAvatar = x.isAvatar,
                                Path = x.Path
                            }).ToList(),
                            CategoryId= vm.CategoryId,
                            CategoryName= vm.CategoryName,
                            Description= vm.Description,
                            PriceOfService = vm.PriceOfService,
                            ServiceName = vm.ServiceName,
                            Status = mappingService.Status,
                            tagofServices = mappingService.TagServices.Select(x=>new TagofServiceViewModel
                            {
                                TagId = x.TagId.ToString(),
                                TagName = _tagServiceRepository.FindSingleAsync(t=>t.Id == x.TagId).Result.TagName
                            }).ToList(), 
                        }
                    };
                }
                else
                {
                    await Logging<RegisterServiceFromProviderCommand>.
                        WarningAsync(ActionCommand.COMMAND_ADD, userName, ErrorMessageConstant.ERROR_ADD_PERMISSION);
                    return new CommandResult<PostServiceViewModel>
                    {
                        isValid = false,
                        errorMessage = ErrorMessageConstant.ERROR_ADD_PERMISSION
                    };
                }
            }
            catch (System.Exception ex)
            {
                await Logging<RegisterServiceFromProviderCommand>.
                        ErrorAsync(ex, ActionCommand.COMMAND_ADD, userName, "Had error");
                return new CommandResult<PostServiceViewModel>
                {
                    isValid = true,
                    myModel = vm,
                    errorMessage = ex.InnerException.ToString()
                };
            }
        }

        private Service MappingService(PostServiceViewModel vm)
        {
            Service sv = new Service();
            sv.CategoryId = vm.CategoryId;
            sv.DateCreated = DateTime.Now;
            sv.PriceOfService = vm.PriceOfService;
            sv.Description = vm.Description;
            sv.ServiceName = vm.ServiceName;
            sv.Status = Status.Pending;
            sv.ServiceImages = vm.listImages.Select(x => new ServiceImage
            {
                Path = x.Path != null ? x.Path : "",
                DateCreated = DateTime.Now,
            }).ToList();

            sv.TagServices = vm.tagofServices.Where(x => x.isDelete == false && x.isAdd == false).Select(x => new Model.Entities.ServiceModel.TagService
            {
                TagId = Guid.Parse(x.TagId),
            }).ToList();
            return sv;
        }

        private Model.Entities.ServiceModel.ProviderServiceModel.ProviderService MappingProviderService(Guid serviceId, Guid idProvider)
        {
            Model.Entities.ServiceModel.ProviderServiceModel.ProviderService providerService = new Model.Entities.ServiceModel.ProviderServiceModel.ProviderService();
            providerService.ProviderId = idProvider;
            providerService.ServiceId = serviceId;
            return providerService;
        }
    }
}