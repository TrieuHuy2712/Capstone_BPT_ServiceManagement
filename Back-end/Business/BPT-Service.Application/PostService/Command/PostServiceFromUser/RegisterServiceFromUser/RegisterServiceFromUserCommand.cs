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

                var mappingService = MappingService(vm);
                await _postServiceRepository.Add(mappingService);

                var mappingUserService = MappingUserService(mappingService.Id, Guid.Parse(userId));
                await _userServiceRepository.Add(mappingUserService);

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
                    isValid = false,
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

            sv.TagServices = vm.tagofServices.Where(x=>x.isDelete==false && x.isAdd ==false).Select(x => new Model.Entities.ServiceModel.TagService
            {
                TagId = x.TagId,
            }).ToList();
            return sv;
        }
        
        private Model.Entities.ServiceModel.UserServiceModel.UserService MappingUserService(Guid serviceId, Guid idUser) {
            Model.Entities.ServiceModel.UserServiceModel.UserService userService = new Model.Entities.ServiceModel.UserServiceModel.UserService();
            userService.UserId = idUser;
            userService.ServiceId = serviceId;
            return userService;
        }
    }
}