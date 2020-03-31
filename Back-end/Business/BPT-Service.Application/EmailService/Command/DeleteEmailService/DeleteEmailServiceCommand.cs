using BPT_Service.Application.PermissionService.Query.CheckUserIsAdmin;
using BPT_Service.Application.PermissionService.Query.GetPermissionAction;
using BPT_Service.Common;
using BPT_Service.Common.Helpers;
using BPT_Service.Model.Entities;
using BPT_Service.Model.Infrastructure.Interfaces;
using Microsoft.AspNetCore.Http;
using System;
using System.Threading.Tasks;

namespace BPT_Service.Application.EmailService.Command.DeleteEmailService
{
    public class DeleteEmailServiceCommand : IDeleteEmailServiceCommand
    {
        private readonly IRepository<Email, int> _emailRepository;
        private readonly ICheckUserIsAdminQuery _checkUserIsAdminQuery;
        private readonly IGetPermissionActionQuery _getPermissionActionQuery;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public DeleteEmailServiceCommand(IRepository<Email, int> emailRepository,
            ICheckUserIsAdminQuery checkUserIsAdminQuery,
            IGetPermissionActionQuery getPermissionActionQuery,
            IHttpContextAccessor httpContextAccessor)
        {
            _emailRepository = emailRepository;
            _checkUserIsAdminQuery = checkUserIsAdminQuery;
            _getPermissionActionQuery = getPermissionActionQuery;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<CommandResult<Email>> ExecuteAsync(int id)
        {
            try
            {
                var userId = _httpContextAccessor.HttpContext.User.Identity.Name;
                if (await _checkUserIsAdminQuery.ExecuteAsync(userId) || await _getPermissionActionQuery.ExecuteAsync(userId, "EMAIL", ActionSetting.CanDelete))
                {
                    var idEmail = await _emailRepository.FindByIdAsync(id);
                    if (idEmail == null)
                    {
                        return new CommandResult<Email>
                        {
                            isValid = false,
                            errorMessage = ErrorMessageConstant.ERROR_CANNOT_FIND_ID
                        };
                    }
                    _emailRepository.Remove(id);
                    await _emailRepository.SaveAsync();
                    return new CommandResult<Email>
                    {
                        isValid = true,
                        myModel = idEmail
                    };
                }
                else
                {
                    return new CommandResult<Email>
                    {
                        isValid = false,
                        errorMessage = ErrorMessageConstant.ERROR_DELETE_PERMISSION
                    };
                }
            }
            catch (Exception ex)
            {
                return new CommandResult<Email>
                {
                    isValid = false,
                    errorMessage = ex.InnerException.Message.ToString()
                };
            }
        }
    }
}