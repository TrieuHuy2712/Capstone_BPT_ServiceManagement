using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BPT_Service.Application.PostService.ViewModel;
using BPT_Service.Application.ProviderService.Query.GetByIdProviderService;
using BPT_Service.Application.TagService.Command.AddServiceAsync;
using BPT_Service.Model.Entities;
using BPT_Service.Model.Entities.ServiceModel;
using BPT_Service.Model.Enums;
using BPT_Service.Model.Infrastructure.Interfaces;
using Microsoft.AspNetCore.Http;

namespace BPT_Service.Application.PostService.Command.PostServiceFromProvider.RegisterServiceFromProvider
{
    public class RegisterServiceFromProviderCommand : IRegisterServiceFromProviderCommand
    {
        private readonly IRepository<Service, Guid> _postServiceRepository;
        private readonly IRepository<ServiceImage, int> _imageServiceRepository;
        private readonly IRepository<Tag, Guid> _tagServiceRepository;
        private readonly IRepository<Model.Entities.ServiceModel.ProviderServiceModel.ProviderService, int> _providerServiceRepository;
        private readonly IGetByIdProviderServiceQuery _getIdProvider;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public RegisterServiceFromProviderCommand(IRepository<Service, Guid> postServiceRepository
        , IRepository<Model.Entities.ServiceModel.ProviderServiceModel.ProviderService, int> providerServiceRepository,
        IRepository<ServiceImage, int> imageServiceRepository,
        IRepository<Tag, Guid> tagServiceRepository,
        IGetByIdProviderServiceQuery getIdProvider,
        IHttpContextAccessor httpContextAccessor)
        {
            _postServiceRepository = postServiceRepository;
            _providerServiceRepository = providerServiceRepository;
            _imageServiceRepository = imageServiceRepository;
            _getIdProvider = getIdProvider;
            _httpContextAccessor = httpContextAccessor;
            _tagServiceRepository = tagServiceRepository;
        }

        public async Task<CommandResult<PostServiceViewModel>> ExecuteAsync(PostServiceViewModel vm)
        {
            try
            {
                var userId = _httpContextAccessor.HttpContext.User.Identity.Name;
                if (userId == null)
                {
                    return new CommandResult<PostServiceViewModel>
                    {
                        isValid = false,
                        myModel = vm
                    };
                }

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

                //Get Id of Provider
                var getIdProvider = await _getIdProvider.ExecuteAsync(userId);

                //Mapping between ViewModel and Model of Service
                var mappingService = MappingService(vm);
                await _postServiceRepository.Add(mappingService);

                //Mapping between ViewModel and Model of ServiceProvider
                var mappingProviderService = MappingProviderService(mappingService.Id, Guid.Parse(getIdProvider.myModel.Id));
                await _providerServiceRepository.Add(mappingProviderService);

                //Add new Tag with Id in TagService
                foreach (var item in newTag)
                {
                    Model.Entities.ServiceModel.TagService mappingTag = new Model.Entities.ServiceModel.TagService();
                    mappingTag.TagId = item.Id;
                    mappingService.TagServices.Add(mappingTag);
                }

                await _tagServiceRepository.SaveAsync();

                return new CommandResult<PostServiceViewModel>
                {
                    isValid = true,
                    myModel = vm
                };
            }
            catch (System.Exception ex)
            {
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
                Path = x.Path,
                DateCreated = DateTime.Now,
            }).ToList();

            
            sv.TagServices = vm.tagofServices.Where(x=>x.isDelete == false && x.isAdd==false).Select(x => new Model.Entities.ServiceModel.TagService
            {
                TagId = x.TagId,
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