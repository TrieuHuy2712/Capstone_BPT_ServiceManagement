using System;
using System.Security.Claims;
using System.Threading.Tasks;
using BPT_Service.Application.ProviderService.ViewModel;
using BPT_Service.Model.Entities;
using BPT_Service.Model.Entities.ServiceModel;
using BPT_Service.Model.Infrastructure.Interfaces;
using Microsoft.AspNetCore.Http;

namespace BPT_Service.Application.ProviderService.Command.DeleteProviderService
{
    public class DeleteProviderServiceCommand : IDeleteProviderServiceCommand
    {
        private readonly IRepository<Provider, Guid> _providerRepository;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public DeleteProviderServiceCommand(IHttpContextAccessor httpContextAccessor,
        IRepository<Provider, Guid> providerRepository)
        {
            _httpContextAccessor = httpContextAccessor;
            _providerRepository = providerRepository;
        }
        public async Task<CommandResult<Provider>> ExecuteAsync(Guid id)
        {
             var userId = _httpContextAccessor.HttpContext.User.Identity.Name;
            if (userId == null)
            {
                return new CommandResult<Provider>
                {
                    isValid = false,
                    myModel = null
                };
            }
            var getId = await _providerRepository.FindByIdAsync(id);
            if (getId != null && getId.AppUser.Id == Guid.Parse(userId))
            {
                _providerRepository.Remove(id);
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
                    myModel = null
                };
            }
        }
    }
}