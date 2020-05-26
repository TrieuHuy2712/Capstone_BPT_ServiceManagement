using BPT_Service.Application.EmailService.Query.GetAllEmailService;
using BPT_Service.Application.PermissionService.Query.CheckUserIsAdmin;
using BPT_Service.Application.PermissionService.Query.GetPermissionAction;
using BPT_Service.Application.PostService.ViewModel;
using BPT_Service.Application.ProviderService.Query.CheckUserIsProvider;
using BPT_Service.Common;
using BPT_Service.Common.Constants;
using BPT_Service.Common.Constants.EmailConstant;
using BPT_Service.Common.Dtos;
using BPT_Service.Common.Helpers;
using BPT_Service.Common.Logging;
using BPT_Service.Model.Entities;
using BPT_Service.Model.Entities.ServiceModel;
using BPT_Service.Model.Enums;
using BPT_Service.Model.Infrastructure.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using SendGrid;
using SendGrid.Helpers.Mail;
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
        private readonly IConfiguration _configuration;
        private readonly IGetAllEmailServiceQuery _getAllEmailServiceQuery;
        private readonly IGetPermissionActionQuery _getPermissionActionQuery;
        private readonly IHttpContextAccessor _httpContext;
        private readonly IOptions<EmailConfigModel> _configEmail;
        private readonly IRepository<Model.Entities.ServiceModel.ProviderServiceModel.ProviderService, int> _providerServiceRepository;
        private readonly IRepository<Model.Entities.ServiceModel.TagService, int> _serviceOfTagRepository;
        private readonly IRepository<Model.Entities.ServiceModel.UserServiceModel.UserService, int> _userServiceRepository;
        private readonly IRepository<Provider, Guid> _providerRepository;
        private readonly IRepository<Service, Guid> _serviceRepository;
        private readonly IRepository<ServiceImage, int> _serviceImageRepository;
        private readonly IRepository<Tag, Guid> _tagServiceRepository;
        private readonly UserManager<AppUser> _userManager;

        public UpdatePostServiceCommand(
            ICheckUserIsAdminQuery checkUserIsAdminQuery, 
            ICheckUserIsProviderQuery checkUserIsProvider, 
            IConfiguration configuration, 
            IGetAllEmailServiceQuery getAllEmailServiceQuery, 
            IGetPermissionActionQuery getPermissionActionQuery, 
            IHttpContextAccessor httpContext, 
            IOptions<EmailConfigModel> configEmail, 
            IRepository<Model.Entities.ServiceModel.ProviderServiceModel.ProviderService, int> providerServiceRepository, 
            IRepository<Model.Entities.ServiceModel.TagService, int> serviceOfTagRepository, 
            IRepository<Model.Entities.ServiceModel.UserServiceModel.UserService, int> userServiceRepository, 
            IRepository<Provider, Guid> providerRepository, 
            IRepository<Service, Guid> serviceRepository, 
            IRepository<ServiceImage, int> serviceImageRepository, 
            IRepository<Tag, Guid> tagServiceRepository, 
            UserManager<AppUser> userManager)
        {
            _checkUserIsAdminQuery = checkUserIsAdminQuery;
            _checkUserIsProvider = checkUserIsProvider;
            _configuration = configuration;
            _getAllEmailServiceQuery = getAllEmailServiceQuery;
            _getPermissionActionQuery = getPermissionActionQuery;
            _httpContext = httpContext;
            _configEmail = configEmail;
            _providerServiceRepository = providerServiceRepository;
            _serviceOfTagRepository = serviceOfTagRepository;
            _userServiceRepository = userServiceRepository;
            _providerRepository = providerRepository;
            _serviceRepository = serviceRepository;
            _serviceImageRepository = serviceImageRepository;
            _tagServiceRepository = tagServiceRepository;
            _userManager = userManager;
        }

        public async Task<CommandResult<PostServiceViewModel>> ExecuteAsync(PostServiceViewModel vm)
        {
            var userId = _httpContext.HttpContext.User.Identity.Name;
            var userName = _userManager.FindByIdAsync(userId).Result.UserName;
            try
            {
                var getPermissionForService = await IsOwnService(Guid.Parse(vm.Id));

                if (getPermissionForService.isValid || await _checkUserIsAdminQuery.ExecuteAsync(userId)
                    || await _getPermissionActionQuery.ExecuteAsync(userId, ConstantFunctions.SERVICE, ActionSetting.CanUpdate))
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
                    //Update Image
                    List<ServiceImage> lstService = new List<ServiceImage>();
                    foreach (var item in vm.listImages)
                    {
                        if (item.ImageId == 0)
                        {
                            ServiceImage serviceImage = new ServiceImage()
                            {
                                DateCreated = DateTime.Now,
                                Path = item.Path,
                                isAvatar = item.IsAvatar,
                                ServiceId = Guid.Parse(vm.Id)
                            };
                            lstService.Add(serviceImage);
                        }
                        else
                        {
                            var imageId = await _serviceImageRepository.FindByIdAsync(item.ImageId);
                            if (imageId != null)
                            {
                                imageId.isAvatar = item.IsAvatar;
                                imageId.DateModified = DateTime.Now;
                                _serviceImageRepository.Update(imageId);
                            }
                        }
                    }
                    var deletImage = await RemoveImage(Guid.Parse(vm.Id), vm.listImages);
                    _serviceImageRepository.RemoveMultiple(deletImage);
                    await _tagServiceRepository.Add(newTag);
                    _tagServiceRepository.RemoveMultiple(tagDelete);
                    var mappingService = await MappingService(vm, service, userId);
                    mappingService.TagServices = null;
                    foreach (var tag in newTag)
                    {
                        Model.Entities.ServiceModel.TagService mappingTag = new Model.Entities.ServiceModel.TagService();
                        mappingTag.TagId = tag.Id;
                        mappingTag.ServiceId = Guid.Parse(vm.Id);
                        await _serviceOfTagRepository.Add(mappingTag);
                    }

                    _serviceRepository.Update(mappingService);
                    await _serviceRepository.SaveAsync();
                    //Cotentemail
                    var findEmailUser = await GetEmailUserAsync(mappingService);
                    if (findEmailUser != ErrorMessageConstant.ERROR_CANNOT_FIND_ID)
                    {
                        //Set content for email
                        //Get All email
                        var getAllEmail = await _getAllEmailServiceQuery.ExecuteAsync();
                        var getFirstEmail = getAllEmail.Where(x => x.Name == EmailName.Approve_Service).FirstOrDefault();

                        var generateCode = _configuration.GetSection("Host").GetSection("LinkConfirmService").Value +
                            mappingService.codeConfirm + '_' + mappingService.Id;

                        getFirstEmail.Message = getFirstEmail.Message.
                            Replace(EmailKey.ServiceNameKey, mappingService.ServiceName).
                            Replace(EmailKey.UserNameKey, findEmailUser).
                            Replace(EmailKey.ConfirmLink, generateCode);
                        ContentEmail(_configEmail.Value.SendGridKey, getFirstEmail.Subject,
                                        getFirstEmail.Message, findEmailUser).Wait();
                    }
                    else
                    {
                        await Logging<UpdatePostServiceCommand>.
                            WarningAsync(ActionCommand.COMMAND_APPROVE, userName, "Cannot find email user");
                        return new CommandResult<PostServiceViewModel>
                        {
                            isValid = false,
                            errorMessage = "Cannot find email user"
                        };
                    }
                    await Logging<UpdatePostServiceCommand>.
                        InformationAsync(ActionCommand.COMMAND_UPDATE, userName, JsonConvert.SerializeObject(vm));
                    return new CommandResult<PostServiceViewModel>
                    {
                        isValid = true,
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
            var userService = await _userServiceRepository.FindSingleAsync(x => x.ServiceId == idService && x.UserId == Guid.Parse(getCurrentId));
            if (userService == null)
            {
                var checkIsProvider = await _providerRepository.FindSingleAsync(x => x.UserId == Guid.Parse(getCurrentId));
                if (checkIsProvider == null)
                {
                    return new CommandResult<Service>
                    {
                        isValid = false,
                        errorMessage = "You don't have permission"
                    };
                }
                else
                {
                    var providerService = _providerServiceRepository.
                        FindSingleAsync(x => x.ProviderId == checkIsProvider.Id && x.ServiceId == idService);
                    if (providerService == null)
                    {
                        return new CommandResult<Service>
                        {
                            isValid = false,
                            errorMessage = "You don't have permission"
                        };
                    }
                }
            }

            return new CommandResult<Service>
            {
                isValid = true,
                errorMessage = "You have permission"
            };
        }

        private async Task<Service> MappingService(PostServiceViewModel vm, Service sv, string currentUserContext)
        {
            sv.CategoryId = vm.CategoryId;
            sv.DateModified = DateTime.Now;
            sv.PriceOfService = vm.PriceOfService;
            sv.Description = vm.Description;
            sv.ServiceName = vm.ServiceName;
            sv.Status = (await _getPermissionActionQuery.ExecuteAsync(currentUserContext, ConstantFunctions.SERVICE, ActionSetting.CanCreate)
                || await _checkUserIsAdminQuery.ExecuteAsync(currentUserContext)) ? Status.WaitingApprove : Status.Pending;
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
        private async Task<string> GetEmailUserAsync(Service service)
        {
            var informationUserService = await _userServiceRepository.FindSingleAsync(x => x.ServiceId == service.Id);
            if (informationUserService != null)
            {
                var getUser = await _userManager.FindByIdAsync(informationUserService.UserId.ToString());
                return getUser.Email;
            }
            var informationProviderService = await _providerServiceRepository.FindSingleAsync(x => x.ServiceId == service.Id);
            if (informationProviderService != null)
            {
                var getProvider = await _providerRepository.FindSingleAsync(x => x.Id == informationProviderService.ProviderId);
                var getUser = await _userManager.FindByIdAsync(getProvider.UserId.ToString());
                return getUser.Email;
            }
            return ErrorMessageConstant.ERROR_CANNOT_FIND_ID;
        }
        private async Task ContentEmail(string apiKey, string subject1, string message, string email)
        {
            var client = new SendGridClient(apiKey);
            var from = new EmailAddress(_configEmail.Value.FromUserEmail, _configEmail.Value.FullUserName);
            var subject = subject1;
            var to = new EmailAddress(email);
            var plainTextContent = message;
            var htmlContent = "<strong>" + message + "</strong>";
            var msg = MailHelper.CreateSingleEmail(from, to, subject, plainTextContent, htmlContent);
            var response = await client.SendEmailAsync(msg);
        }
    }
}