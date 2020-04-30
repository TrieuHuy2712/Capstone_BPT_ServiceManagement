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
using BPT_Service.Common.Support;
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
        private readonly IGetAllEmailServiceQuery _getAllEmailServiceQuery;
        private readonly IOptions<EmailConfigModel> _config;
        private readonly IConfiguration _configuration;

        public RegisterServiceFromUserCommand(
            ICheckUserIsAdminQuery checkUserIsAdminQuery,
            ICheckUserIsProviderQuery checkUserIsProvider,
            IGetPermissionActionQuery getPermissionActionQuery,
            IHttpContextAccessor httpContextAccessor,
            IRepository<Model.Entities.ServiceModel.UserServiceModel.UserService, int> userServiceRepository,
            IRepository<Service, Guid> postServiceRepository,
            IRepository<Tag, Guid> tagServiceRepository,
            UserManager<AppUser> userManager,
            IGetAllEmailServiceQuery getAllEmailServiceQuery,
            IOptions<EmailConfigModel> config,
             IConfiguration configuration)
        {
            _checkUserIsAdminQuery = checkUserIsAdminQuery;
            _checkUserIsProvider = checkUserIsProvider;
            _getPermissionActionQuery = getPermissionActionQuery;
            _httpContextAccessor = httpContextAccessor;
            _userServiceRepository = userServiceRepository;
            _postServiceRepository = postServiceRepository;
            _tagServiceRepository = tagServiceRepository;
            _userManager = userManager;
            _getAllEmailServiceQuery = getAllEmailServiceQuery;
            _config = config;
            _configuration = configuration;
        }

        public async Task<CommandResult<PostServiceViewModel>> ExecuteAsync(PostServiceViewModel vm)
        {
            var userId = _httpContextAccessor.HttpContext.User.Identity.Name;
            var userName = _userManager.FindByIdAsync(userId).Result.UserName;
            try
            {
                //var checkUserIsAdmin = await _checkUserIsAdminQuery.ExecuteAsync(userId);
                if (await _getPermissionActionQuery.ExecuteAsync(userId, ConstantFunctions.SERVICE, ActionSetting.CanCreate) ||
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
                    var mappingService = await MappingService(vm, userId);
                    await _postServiceRepository.Add(mappingService);

                    //Mapping between ViewModel and Model of UserService
                    var mappingUserService = MappingUserService(mappingService.Id, !string.IsNullOrEmpty(vm.UserId) ? Guid.Parse(vm.UserId) : Guid.Parse(userId));
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
                    var findUserInformation = await _userManager.FindByIdAsync(vm.UserId);
                    //Send mail for user if admin
                    if ((await _getPermissionActionQuery.ExecuteAsync(userId, ConstantFunctions.SERVICE, ActionSetting.CanCreate)
                || await _checkUserIsAdminQuery.ExecuteAsync(userId)))
                    {
                        //Set content for email
                        //Generate code
                        //Create Generate code
                        var generateCode = _configuration.GetSection("Host").GetSection("LinkConfirmService") +
                         mappingService.codeConfirm + '_' + mappingService.Id;

                        var getEmailContent = await _getAllEmailServiceQuery.ExecuteAsync();
                        var getFirstEmail = getEmailContent.Where(x => x.Name == EmailName.Approve_Service).FirstOrDefault();
                        getFirstEmail.Message = getFirstEmail.Message.
                            Replace(EmailKey.UserNameKey, findUserInformation.Email).
                            Replace(EmailKey.ConfirmLink, generateCode); ;

                        ContentEmail(_config.Value.SendGridKey, getFirstEmail.Subject,
                                        getFirstEmail.Message, _userManager.FindByIdAsync(!string.IsNullOrEmpty(vm.UserId) ? vm.UserId : userId).Result.Email).Wait();
                    }
                    //End send mail for user
                    return new CommandResult<PostServiceViewModel>
                    {
                        isValid = true,
                        myModel = new PostServiceViewModel
                        {
                            Id = mappingService.Id.ToString(),
                            DateCreated = mappingService.DateCreated,
                            IsProvider = false,
                            Author = findUserInformation.UserName + "(" + findUserInformation.Email + ")",
                            UserId = vm.UserId,
                            AvtService = mappingService.ServiceImages.Where(x => x.isAvatar == true).FirstOrDefault().Path,
                            listImages = mappingService.ServiceImages.Select(x => new PostServiceImageViewModel
                            {
                                ImageId = x.Id,
                                IsAvatar = x.isAvatar,
                                Path = x.Path
                            }).ToList(),
                            CategoryId = vm.CategoryId,
                            CategoryName = vm.CategoryName,
                            Description = vm.Description,
                            PriceOfService = vm.PriceOfService,
                            ServiceName = vm.ServiceName,
                            Status = mappingService.Status,
                            tagofServices = mappingService.TagServices.Select(x => new TagofServiceViewModel
                            {
                                TagId = x.TagId.ToString(),
                                TagName = _tagServiceRepository.FindSingleAsync(t => t.Id == x.TagId).Result.TagName
                            }).ToList(),
                        }
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
            sv.codeConfirm = RandomCodeSupport.RandomString(6);
            sv.Status = (await _getPermissionActionQuery.ExecuteAsync(currentUserContext, ConstantFunctions.SERVICE, ActionSetting.CanCreate)
                || await _checkUserIsAdminQuery.ExecuteAsync(currentUserContext)) ? Status.WaitingApprove : Status.Pending;
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

        private async Task ContentEmail(string apiKey, string subject1, string message, string email)
        {
            var client = new SendGridClient(apiKey);
            var from = new EmailAddress(_config.Value.FromUserEmail, _config.Value.FullUserName);
            var subject = subject1;
            var to = new EmailAddress(email);
            var plainTextContent = message;
            var htmlContent = "<strong>" + message + "</strong>";
            var msg = MailHelper.CreateSingleEmail(from, to, subject, plainTextContent, htmlContent);
            var response = await client.SendEmailAsync(msg);
        }
    }
}