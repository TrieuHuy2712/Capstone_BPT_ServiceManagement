using BPT_Service.Application.EmailService.Query.GetAllEmailService;
using BPT_Service.Application.PermissionService.Query.CheckUserIsAdmin;
using BPT_Service.Application.PermissionService.Query.GetPermissionAction;
using BPT_Service.Common;
using BPT_Service.Common.Helpers;
using BPT_Service.Model.Entities;
using BPT_Service.Model.Entities.ServiceModel;
using BPT_Service.Model.Infrastructure.Interfaces;
using Microsoft.AspNetCore.Http;
using System;
using System.Threading.Tasks;

namespace BPT_Service.Application.ProviderService.Command.DeleteProviderService
{
    public class DeleteProviderServiceCommand : IDeleteProviderServiceCommand
    {
        private readonly IRepository<Provider, Guid> _providerRepository;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IGetPermissionActionQuery _getPermissionActionQuery;
        private readonly ICheckUserIsAdminQuery _checkUserIsAdminQuery;

        public DeleteProviderServiceCommand(
            IRepository<Provider, Guid> providerRepository, 
            IHttpContextAccessor httpContextAccessor, 
            IGetPermissionActionQuery getPermissionActionQuery, 
            ICheckUserIsAdminQuery checkUserIsAdminQuery)
        {
            _providerRepository = providerRepository;
            _httpContextAccessor = httpContextAccessor;
            _getPermissionActionQuery = getPermissionActionQuery;
            _checkUserIsAdminQuery = checkUserIsAdminQuery;
        }

        public async Task<CommandResult<Provider>> ExecuteAsync(string id)
        {
            try
            {
                var newId = Guid.Parse(id);
                var userId = _httpContextAccessor.HttpContext.User.Identity.Name;
                if(await _checkUserIsAdminQuery.ExecuteAsync(userId) || await _getPermissionActionQuery.ExecuteAsync(userId,"PROVIDER", ActionSetting.CanDelete))
                {
                    var getId = await _providerRepository.FindByIdAsync(newId);
                    if (getId != null)
                    {
                        //Remove Provider Role
                        _providerRepository.Remove(newId);
                        await _providerRepository.SaveAsync();
                        return new CommandResult<Provider>
                        {
                            isValid = true,
                            myModel = getId
                        };
                    }
                    else
                    {
                        return new CommandResult<Provider>
                        {
                            isValid = false,
                            errorMessage = ErrorMessageConstant.ERROR_CANNOT_FIND_ID
                        };
                    }
                }
                else
                {
                    return new CommandResult<Provider>
                    {
                        isValid = false,
                        errorMessage = ErrorMessageConstant.ERROR_DELETE_PERMISSION
                    };
                }
                
            }
            catch (System.Exception ex)
            {
                return new CommandResult<Provider>
                {
                    isValid = false,
                    errorMessage = ex.InnerException.ToString()
                };
            }
        }
    }
}