using System;
using System.Threading.Tasks;
using BPT_Service.Application.PostService.ViewModel;
using BPT_Service.Model.Entities;
using BPT_Service.Model.Entities.ServiceModel;
using BPT_Service.Model.Infrastructure.Interfaces;
using Microsoft.AspNetCore.Http;

namespace BPT_Service.Application.PostService.Command.PostServiceFromProvider.DeleteServiceFromProvider
{
    public class DeleteServiceFromProviderCommand : IDeleteServiceFromProviderCommand
    {
        private readonly IRepository<Service, Guid> _postServiceRepository;
        private readonly IRepository<Provider, Guid> _providerRepository;
        private readonly IRepository<Model.Entities.ServiceModel.ProviderServiceModel.ProviderService, int> _providerServiceRepository;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public DeleteServiceFromProviderCommand(IRepository<Service, Guid> postServiceRepository,
        IRepository<Provider, Guid> providerRepository,
        IRepository<Model.Entities.ServiceModel.ProviderServiceModel.ProviderService, int> providerServiceRepository,
        IHttpContextAccessor httpContextAccessor)
        {
            _postServiceRepository = postServiceRepository;
            _providerRepository = providerRepository;
            _providerServiceRepository = providerServiceRepository;
            _httpContextAccessor = httpContextAccessor;
        }
        public async Task<CommandResult<PostServiceViewModel>> ExecuteAsync(Guid idService)
        {
            try
            {
                var userId = Guid.Parse(_httpContextAccessor.HttpContext.User.Identity.Name);
                var getService = await _postServiceRepository.FindByIdAsync(idService);
                if (getService == null)
                {
                    return new CommandResult<PostServiceViewModel>
                    {
                        isValid = false,
                        errorMessage = "Cannot find your Service"
                    };
                }

                var getProviderService = await _providerServiceRepository.FindSingleAsync(x => x.ServiceId == getService.Id);
                if (getProviderService == null)
                {
                    return new CommandResult<PostServiceViewModel>
                    {
                        isValid = false,
                        errorMessage = "Cannot find your ProviderService"
                    };
                }

                var getProvider = await _providerRepository.FindSingleAsync(x => x.UserId == userId);
                if (getProvider == null)
                {
                    return new CommandResult<PostServiceViewModel>
                    {
                        isValid = false,
                        errorMessage = "Cannot find your UserId"
                    };
                }

                if (getProvider.Id == getProviderService.ProviderId)
                {
                    _postServiceRepository.Remove(getService);
                    await _postServiceRepository.SaveAsync();
                    return new CommandResult<PostServiceViewModel>
                    {
                        isValid = true,
                    };
                }
                return new CommandResult<PostServiceViewModel>
                {
                    isValid = false,
                    errorMessage = "Cannot find your information"
                };
            }
            catch (System.Exception ex)
            {
                return new CommandResult<PostServiceViewModel>
                {
                    isValid = false,
                    errorMessage = ex.InnerException.ToString()
                };
            }
        }
    }
}