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
using System.Security.Claims;
using System.Threading.Tasks;

namespace BPT_Service.Application.PostService.Command.PostServiceFromUser.RegisterServiceFromUser
{
    public class RegisterServiceFromUserCommand : IRegisterServiceFromUserCommand
    {
        private readonly ICheckUserIsAdminQuery _checkUserIsAdminQuery;
        private readonly ICheckUserIsProviderQuery _checkUserIsProvider;
        private readonly IGetPermissionActionQuery _getPermissionActionQuery;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IRepository<Model.Entities.ServiceModel.UserServiceModel.UserService, int> _userServiceRepository;
        private readonly IRepository<Service, Guid> _postServiceRepository;
        private readonly IRepository<Tag, Guid> _tagServiceRepository;
        private readonly UserManager<AppUser> _userManager;

        public RegisterServiceFromUserCommand(
            IRepository<Service, Guid> postServiceRepository,
            IRepository<Tag, Guid> tagServiceRepository,
            IRepository<Model.Entities.ServiceModel.UserServiceModel.UserService, int> userServiceRepository,
            IHttpContextAccessor httpContextAccessor,
            ICheckUserIsProviderQuery checkUserIsProvider,
            ICheckUserIsAdminQuery checkUserIsAdminQuery,
            IGetPermissionActionQuery getPermissionActionQuery,
            UserManager<AppUser> userManager)
        {
            _postServiceRepository = postServiceRepository;
            _tagServiceRepository = tagServiceRepository;
            _userServiceRepository = userServiceRepository;
            _httpContextAccessor = httpContextAccessor;
            _checkUserIsProvider = checkUserIsProvider;
            _checkUserIsAdminQuery = checkUserIsAdminQuery;
            _getPermissionActionQuery = getPermissionActionQuery;
            _userManager = userManager;
        }

        public async Task<CommandResult<PostServiceViewModel>> ExecuteAsync(PostServiceViewModel vm)
        {
            var userId = _httpContextAccessor.HttpContext.User.Identity.Name;
            var userName = _userManager.FindByIdAsync(userId).Result.UserName;
            try
            {
                var checkUserIsAdmin = await _checkUserIsProvider.ExecuteAsync();
                if (await _getPermissionActionQuery.ExecuteAsync(userId, "SERVICE", ActionSetting.CanCreate) ||
                    await _checkUserIsAdminQuery.ExecuteAsync(userId))
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
                    var mappingService = await MappingService(vm,userId);
                    await _postServiceRepository.Add(mappingService);

                    //Mapping between ViewModel and Model of UserService
                    var mappingUserService = MappingUserService(mappingService.Id, Guid.Parse(vm.UserId));
                    await _userServiceRepository.Add(mappingUserService);

                    //Add new Tag with Id in TagService
                    foreach (var item in newTag)
                    {
                        Model.Entities.ServiceModel.TagService mappingTag = new Model.Entities.ServiceModel.TagService();
                        mappingTag.TagId = item.Id;
                        mappingService.TagServices.Add(mappingTag);
                    }

                    await _tagServiceRepository.SaveAsync();
                    //Write Log
                    await Logging<RegisterServiceFromUserCommand>.
                        InformationAsync(ActionCommand.COMMAND_ADD, userName, JsonConvert.SerializeObject(vm));
                    return new CommandResult<PostServiceViewModel>
                    {
                        isValid = true,
                        myModel = vm
                    };
                }
                else
                {
                    await Logging<RegisterServiceFromUserCommand>.
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
                await Logging<RegisterServiceFromUserCommand>.
                        ErrorAsync(ex, ActionCommand.COMMAND_ADD, userName, "Had error");
                return new CommandResult<PostServiceViewModel>
                {
                    isValid = true,
                    myModel = vm,
                    errorMessage = ex.InnerException.ToString()
                };
            }
        }

        private async Task<Service> MappingService(PostServiceViewModel vm, string currentUserContext)
        {
            Service sv = new Service();
            sv.CategoryId = vm.CategoryId;
            sv.DateCreated = DateTime.Now;
            sv.PriceOfService = vm.PriceOfService;
            sv.Description = vm.Description;
            sv.ServiceName = vm.ServiceName;
            sv.Status = (await _getPermissionActionQuery.ExecuteAsync(currentUserContext, "SERVICE", ActionSetting.CanCreate)
                || await _checkUserIsAdminQuery.ExecuteAsync(currentUserContext)) ? Status.Active : Status.Pending;
            sv.ServiceImages = vm.listImages.Select(x => new ServiceImage
            {
                Path = x.Path != null ? x.Path : "",
                DateCreated = DateTime.Now,
                isAvatar = x.IsAvatar
            }).ToList();

            sv.TagServices = vm.tagofServices.Where(x => x.isDelete == false && x.isAdd == false).Select(x => new Model.Entities.ServiceModel.TagService
            {
                TagId = Guid.Parse(x.TagId),
            }).ToList();
            return sv;
        }

        private Model.Entities.ServiceModel.UserServiceModel.UserService MappingUserService(Guid serviceId, Guid idUser)
        {
            Model.Entities.ServiceModel.UserServiceModel.UserService userService = new Model.Entities.ServiceModel.UserServiceModel.UserService();
            userService.UserId = idUser;
            userService.ServiceId = serviceId;
            return userService;
        }
    }
}