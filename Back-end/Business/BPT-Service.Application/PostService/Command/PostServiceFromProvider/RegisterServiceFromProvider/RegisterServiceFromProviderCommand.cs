using BPT_Service.Application.EmailService.Query.GetAllEmailService;
using BPT_Service.Application.PermissionService.Query.CheckUserIsAdmin;
using BPT_Service.Application.PermissionService.Query.GetPermissionAction;
using BPT_Service.Application.PostService.ViewModel;
using BPT_Service.Application.ProviderService.Query.CheckUserIsProvider;
using BPT_Service.Application.ProviderService.Query.GetByIdProviderService;
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

namespace BPT_Service.Application.PostService.Command.PostServiceFromProvider.RegisterServiceFromProvider
{
    public class RegisterServiceFromProviderCommand : IRegisterServiceFromProviderCommand
    {
        private readonly ICheckUserIsAdminQuery _checkUserIsAdminQuery;
        private readonly ICheckUserIsProviderQuery _checkUserIsProvider;
        private readonly IConfiguration _configuration;
        private readonly IGetAllEmailServiceQuery _getAllEmailServiceQuery;
        private readonly IGetByIdProviderServiceQuery _getByIdProviderServiceQuery;
        private readonly IGetPermissionActionQuery _getPermissionActionQuery;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IOptions<EmailConfigModel> _config;
        private readonly IRepository<Model.Entities.ServiceModel.ProviderServiceModel.ProviderService, int> _providerServiceRepository;
        private readonly IRepository<Provider, Guid> _providerRepository;
        private readonly IRepository<Service, Guid> _postServiceRepository;
        private readonly IRepository<Tag, Guid> _tagServiceRepository;
        private readonly UserManager<AppUser> _userManager;

        public RegisterServiceFromProviderCommand(
            ICheckUserIsAdminQuery checkUserIsAdminQuery, 
            ICheckUserIsProviderQuery checkUserIsProvider, 
            IConfiguration configuration, 
            IGetAllEmailServiceQuery getAllEmailServiceQuery, 
            IGetByIdProviderServiceQuery getByIdProviderServiceQuery, 
            IGetPermissionActionQuery getPermissionActionQuery, 
            IHttpContextAccessor httpContextAccessor, 
            IOptions<EmailConfigModel> config, 
            IRepository<Model.Entities.ServiceModel.ProviderServiceModel.ProviderService, int> providerServiceRepository, 
            IRepository<Provider, Guid> providerRepository, 
            IRepository<Service, Guid> postServiceRepository, 
            IRepository<Tag, Guid> tagServiceRepository, 
            UserManager<AppUser> userManager)
        {
            _checkUserIsAdminQuery = checkUserIsAdminQuery;
            _checkUserIsProvider = checkUserIsProvider;
            _configuration = configuration;
            _getAllEmailServiceQuery = getAllEmailServiceQuery;
            _getByIdProviderServiceQuery = getByIdProviderServiceQuery;
            _getPermissionActionQuery = getPermissionActionQuery;
            _httpContextAccessor = httpContextAccessor;
            _config = config;
            _providerServiceRepository = providerServiceRepository;
            _providerRepository = providerRepository;
            _postServiceRepository = postServiceRepository;
            _tagServiceRepository = tagServiceRepository;
            _userManager = userManager;
        }

        public async Task<CommandResult<PostServiceViewModel>> ExecuteAsync(PostServiceViewModel vm)
        {
            var userId = _httpContextAccessor.HttpContext.User.Identity.Name;
            var userName = _userManager.FindByIdAsync(userId).Result.UserName;
            try
            {
                var checkUserIsProvider = await _checkUserIsProvider.ExecuteAsync(userId);
                if (await _getPermissionActionQuery.ExecuteAsync(userId, ConstantFunctions.SERVICE, ActionSetting.CanCreate) ||
                    await _checkUserIsAdminQuery.ExecuteAsync(userId) || checkUserIsProvider.isValid)
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
                        InformationAsync(ActionCommand.COMMAND_ADD, userName, JsonConvert.SerializeObject(vm));
                    //Get user Information
                    var findUserId = await _userManager.FindByIdAsync(GetProvider(vm.ProviderId).Result.UserId.ToString());
                    //Send mail for user
                    if ((await _getPermissionActionQuery.ExecuteAsync(userId, ConstantFunctions.SERVICE, ActionSetting.CanCreate)
                || await _checkUserIsAdminQuery.ExecuteAsync(userId)))
                    {
                        //Create Generate code
                        var generateCode = _configuration.GetSection("Host").GetSection("LinkConfirmService").Value +
                         mappingService.codeConfirm + '_' + mappingService.Id;
                        //Set content for email
                        var getEmailContent = await _getAllEmailServiceQuery.ExecuteAsync();
                        var getFirstEmail = getEmailContent.Where(x => x.Name == EmailName.Approve_Service).FirstOrDefault();
                        getFirstEmail.Message = getFirstEmail.Message.
                            Replace(EmailKey.UserNameKey, findUserId.Email).
                            Replace(EmailKey.ConfirmLink, generateCode);

                        ContentEmail(_config.Value.SendGridKey, getFirstEmail.Subject,
                                        getFirstEmail.Message, findUserId.Email).Wait();
                    }
                    else
                    {
                        var getEmailContent = await _getAllEmailServiceQuery.ExecuteAsync();
                        var getFirstEmail = getEmailContent.Where(x => x.Name == EmailName.Receive_Register_Service).FirstOrDefault();
                        getFirstEmail.Message = getFirstEmail.Message.
                            Replace(EmailKey.UserNameKey, findUserId.Email);
                        ContentEmail(_config.Value.SendGridKey, getFirstEmail.Subject,
                                        getFirstEmail.Message, findUserId.Email).Wait();
                    }
                    //End send mail for user
                    return new CommandResult<PostServiceViewModel>
                    {
                        isValid = true,
                        myModel = new PostServiceViewModel
                        {
                            Id = mappingService.Id.ToString(),
                            DateCreated = mappingService.DateCreated,
                            IsProvider = true,
                            Author = _providerRepository.FindSingleAsync(x => x.Id == Guid.Parse(vm.ProviderId)).Result.ProviderName,
                            ProviderId = vm.ProviderId,
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
            sv.codeConfirm = RandomCodeSupport.RandomString(6);
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

        private Model.Entities.ServiceModel.ProviderServiceModel.ProviderService MappingProviderService(Guid serviceId, Guid idProvider)
        {
            Model.Entities.ServiceModel.ProviderServiceModel.ProviderService providerService = new Model.Entities.ServiceModel.ProviderServiceModel.ProviderService();
            providerService.ProviderId = idProvider;
            providerService.ServiceId = serviceId;
            return providerService;
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

        private async Task<Provider> GetProvider(string idProvider)
        {
            var getProvider = await _providerRepository.FindByIdAsync(Guid.Parse(idProvider));
            return getProvider;
        }
    }
}