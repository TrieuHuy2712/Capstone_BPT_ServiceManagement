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

namespace BPT_Service.Application.PostService.Command.PostServiceFromUser.RegisterServiceFromUser
{
    public class RegisterServiceFromUserCommand : IRegisterServiceFromUserCommand
    {
        private readonly IRepository<Service, Guid> _postServiceRepository;
        private readonly IRepository<ServiceImage, int> _imageServiceRepository;
        private readonly IRepository<Tag, Guid> _tagServiceRepository;
        private readonly IRepository<Model.Entities.ServiceModel.UserServiceModel.UserService, int> _userServiceRepository;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public RegisterServiceFromUserCommand(IRepository<Service, Guid> postServiceRepository,
        IRepository<ServiceImage, int> imageServiceRepository,
        IRepository<Tag, Guid> tagServiceRepository,
        IRepository<Model.Entities.ServiceModel.UserServiceModel.UserService, int> userServiceRepository,
        IHttpContextAccessor httpContextAccessor)
        {
            _postServiceRepository = postServiceRepository;
            _imageServiceRepository = imageServiceRepository;
            _httpContextAccessor = httpContextAccessor;
            _tagServiceRepository = tagServiceRepository;
            _userServiceRepository = userServiceRepository;
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

                var mappingService = MappingService(vm, Guid.Parse(userId));
                await _postServiceRepository.Add(mappingService);

                foreach (var item in newTag)
                {
                    Model.Entities.ServiceModel.TagService mappingTag = new Model.Entities.ServiceModel.TagService();
                    mappingTag.TagId = item.Id;
                    mappingService.TagServices.Add(mappingTag);
                }

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

        private Service MappingService(PostServiceViewModel vm, Guid idUser)
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
                ServiceId = x.ServiceId
            }).ToList();

            sv.UserServices = vm.userofServices.Select(x => new Model.Entities.ServiceModel.UserServiceModel.UserService
            {
                ServiceId = x.ServiceId,
                UserId = idUser,
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