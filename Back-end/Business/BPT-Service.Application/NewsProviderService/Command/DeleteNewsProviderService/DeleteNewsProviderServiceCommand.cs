using System;
using System.Threading.Tasks;
using BPT_Service.Model.Entities;
using BPT_Service.Model.Entities.ServiceModel;
using BPT_Service.Model.Entities.ServiceModel.ProviderServiceModel;
using BPT_Service.Model.Infrastructure.Interfaces;
using Microsoft.AspNetCore.Http;

namespace BPT_Service.Application.NewsProviderService.Command.DeleteNewsProviderService
{
    public class DeleteNewsProviderServiceCommand : IDeleteNewsProviderServiceCommand
    {
        private readonly IRepository<ProviderNew, int> _providerNewRepository;
        private readonly IRepository<Provider, Guid> _providerRepository;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public DeleteNewsProviderServiceCommand(IRepository<ProviderNew, int> providerNewRepository, IHttpContextAccessor httpContextAccessor, IRepository<Provider, Guid> providerRepository)
        {
            _providerNewRepository = providerNewRepository;
            _httpContextAccessor = httpContextAccessor;
            _providerRepository = providerRepository;
        }
        public async Task<CommandResult<ProviderNew>> ExecuteAsync(int id)
        {
            try
            {
                var userId = _httpContextAccessor.HttpContext.User.Identity.Name;
                if (userId == null)
                {
                    return new CommandResult<ProviderNew>
                    {
                        isValid = false,
                        myModel = null
                    };
                }
                var getId = await _providerNewRepository.FindByIdAsync(id);
                if (getId != null)
                {
                    var getProvider = await _providerRepository.FindByIdAsync(getId.ProviderId);
                    if (getProvider != null)
                    {
                        var getAllProviderOffUser = await _providerRepository.FindAllAsync(x => x.AppUser.UserName == userId);
                        var counProvider = 0;
                        foreach (var item in getAllProviderOffUser)
                        {
                            if (item.Id == getProvider.Id)
                            {
                                counProvider++;
                            }
                        }
                        if (counProvider > 0)
                        {
                            _providerNewRepository.Remove(id);
                            await _providerNewRepository.SaveAsync();
                            return new CommandResult<ProviderNew>
                            {
                                isValid = true,
                                myModel = getId
                            };
                        }
                        else
                        {
                            return new CommandResult<ProviderNew>
                            {
                                isValid = false,
                                errorMessage = "You don't have permission"
                            };
                        }
                    }
                    else
                    {
                        return new CommandResult<ProviderNew>
                        {
                            isValid = false,
                            errorMessage = "Cannot find your Provider"
                        };
                    }
                }
                return new CommandResult<ProviderNew>
                {
                    isValid = false,
                    errorMessage = "Cannot find Id"
                };

            }
            catch (System.Exception ex)
            {
                return new CommandResult<ProviderNew>
                {
                    isValid = false,
                    errorMessage = ex.InnerException.ToString()
                };
            }
        }
    }
}