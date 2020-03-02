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

                var getIdProvider = _getIdProvider.ExecuteAsync(userId).Result.myModel.Id;
                var mappingService = MappingService(vm, Guid.Parse(getIdProvider));


                foreach (var item in newTag)
                {
                    Model.Entities.ServiceModel.TagService mappingTag = new Model.Entities.ServiceModel.TagService();
                    mappingTag.TagId = item.Id;
                    mappingService.TagServices.Add(mappingTag);
                }
                await _postServiceRepository.Add(mappingService);
                await _tagServiceRepository.SaveAsync();
                await _postServiceRepository.SaveAsync();

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

        private Service MappingService(PostServiceViewModel vm, Guid idProvider)
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

            sv.ProviderServices.ProviderId = idProvider;
            sv.ProviderServices.ServiceId = Guid.Parse(vm.Id);
            sv.TagServices = vm.tagofServices.Select(x => new Model.Entities.ServiceModel.TagService
            {
                TagId = Guid.Parse(x.TagId),
            }).ToList();
            return sv;
        }
    }
}