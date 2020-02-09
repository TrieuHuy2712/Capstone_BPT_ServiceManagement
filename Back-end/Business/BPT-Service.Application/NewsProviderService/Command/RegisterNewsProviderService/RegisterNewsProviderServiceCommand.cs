using System;
using System.Threading.Tasks;
using BPT_Service.Application.NewsProviderService.ViewModel;
using BPT_Service.Model.Entities;
using BPT_Service.Model.Entities.ServiceModel;
using BPT_Service.Model.Entities.ServiceModel.ProviderServiceModel;
using BPT_Service.Model.Enums;
using BPT_Service.Model.Infrastructure.Interfaces;
using Microsoft.AspNetCore.Http;

namespace BPT_Service.Application.NewsProviderService.Command.RegisterNewsProviderService
{
    public class RegisterNewsProviderServiceCommand : IRegisterNewsProviderServiceCommand
    {
        private readonly IRepository<ProviderNew, int> _newProviderRepository;
        private readonly IRepository<Provider, Guid> _providerRepository;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public RegisterNewsProviderServiceCommand(IHttpContextAccessor httpContextAccessor,
        IRepository<ProviderNew, int> newProviderRepository,
        IRepository<Provider, Guid> providerRepository)
        {
            _httpContextAccessor = httpContextAccessor;
            _newProviderRepository = newProviderRepository;
            _providerRepository = providerRepository;
        }
        public async Task<CommandResult<NewsProviderViewModel>> ExecuteAsync(NewsProviderViewModel vm)
        {
            try
            {
                var userId = _httpContextAccessor.HttpContext.User.Identity.Name;
                if (userId == null)
                {
                    return new CommandResult<NewsProviderViewModel>
                    {
                        isValid = false,
                        myModel = vm
                    };
                }
                var checkUser = await _providerRepository.FindAllAsync(x => x.AppUser.UserName == userId);
                var countProvider = 0;
                foreach (var item in checkUser)
                {
                    if (item.Id == vm.ProviderId)
                    {
                        countProvider++;
                    }
                }
                if (countProvider == 0)
                {
                    return new CommandResult<NewsProviderViewModel>
                    {
                        isValid = false,
                        myModel = vm
                    };
                }
                var getProvider = await _providerRepository.FindByIdAsync(vm.ProviderId);
                if (getProvider == null)
                {
                    return new CommandResult<NewsProviderViewModel>
                    {
                        isValid = false,
                        myModel = vm,
                        errorMessage = "You don't have permission with this Provider"
                    };
                }
                var mappingProvider = MappingProvider(vm, getProvider.Id);
                await _newProviderRepository.Add(mappingProvider);
                await _newProviderRepository.SaveAsync();
                vm.Id = mappingProvider.Id;
                return new CommandResult<NewsProviderViewModel>
                {
                    isValid = true,
                    myModel = vm
                };
            }
            catch (System.Exception ex)
            {
                return new CommandResult<NewsProviderViewModel>
                {
                    isValid = false,
                    myModel = vm,
                    errorMessage = ex.InnerException.ToString()
                };
            }
        }
        
        private ProviderNew MappingProvider(NewsProviderViewModel vm, Guid providerId)
        {
            ProviderNew pro = new ProviderNew();
            pro.Id = vm.Id;
            pro.Author = vm.Author;
            pro.Status = Status.Pending;
            pro.Author = vm.Author;
            pro.ProviderId = providerId;
            pro.Title = vm.Title;
            pro.Content = vm.Content;
            pro.DateCreated = DateTime.Now;
            return pro;
        }
    }
}