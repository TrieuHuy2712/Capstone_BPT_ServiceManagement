using BPT_Service.Application.CategoryService.Command.AddCategoryService;
using BPT_Service.Application.EmailService.ViewModel;
using BPT_Service.Application.PermissionService.Query.CheckUserIsAdmin;
using BPT_Service.Application.PermissionService.Query.GetPermissionAction;
using BPT_Service.Common;
using BPT_Service.Common.Helpers;
using BPT_Service.Common.Logging;
using BPT_Service.Model.Entities;
using BPT_Service.Model.Infrastructure.Interfaces;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using System;
using System.Security.Claims;
using System.Threading.Tasks;

namespace BPT_Service.Application.EmailService.Command.AddNewEmailService
{
    public class AddNewEmailServiceCommand : IAddNewEmailServiceCommand
    {
        private readonly IRepository<Email, int> _emailRepository;
        private readonly ICheckUserIsAdminQuery _checkUserIsAdminQuery;
        private readonly IGetPermissionActionQuery _getPermissionActionQuery;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public AddNewEmailServiceCommand(IRepository<Email, int> emailRepository,
            ICheckUserIsAdminQuery checkUserIsAdminQuery,
            IGetPermissionActionQuery getPermissionActionQuery,
            IHttpContextAccessor httpContextAccessor)
        {
            _emailRepository = emailRepository;
            _checkUserIsAdminQuery = checkUserIsAdminQuery;
            _getPermissionActionQuery = getPermissionActionQuery;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<CommandResult<Email>> ExecuteAsync(EmailViewModel emailVIewModel)
        {
            var userName = _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value;
            try
            {
                //Check user has permission first
                var userId = _httpContextAccessor.HttpContext.User.Identity.Name;
                if (await _checkUserIsAdminQuery.ExecuteAsync(userId) || await _getPermissionActionQuery.ExecuteAsync(userId, "EMAIL", ActionSetting.CanCreate))
                {
                    var mappingEmail = MappingEmail(emailVIewModel);
                    await _emailRepository.Add(mappingEmail);
                    await _emailRepository.SaveAsync();
                    await Logging<AddNewEmailServiceCommand>.
                       InformationAsync(ActionCommand.COMMAND_ADD, userName, JsonConvert.SerializeObject(mappingEmail));
                    return new CommandResult<Email>
                    {
                        isValid = true,
                        myModel = mappingEmail
                    };
                }
                else
                {
                    await Logging<AddNewEmailServiceCommand>.
                        WarningAsync(ActionCommand.COMMAND_ADD, userName, ErrorMessageConstant.ERROR_ADD_PERMISSION);
                    return new CommandResult<Email>
                    {
                        isValid = false,
                        errorMessage = ErrorMessageConstant.ERROR_ADD_PERMISSION
                    };
                }
            }
            catch (Exception ex)
            {
                await Logging<AddNewEmailServiceCommand>.ErrorAsync(ex, ActionCommand.COMMAND_ADD, userName, "You have error");
                return new CommandResult<Email>
                {
                    isValid = false,
                    errorMessage = ex.InnerException.Message.ToString()
                };
            }
        }

        private Email MappingEmail(EmailViewModel emailViewModel)
        {
            Email email = new Email();
            email.Message = emailViewModel.Message;
            email.Name = emailViewModel.Name;
            email.Subject = emailViewModel.Subject;
            email.To = emailViewModel.To;
            return email;
        }
    }
}