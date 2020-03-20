using System;
using System.Security.Claims;
using System.Threading.Tasks;
using BPT_Service.Application.ProviderService.ViewModel;
using BPT_Service.Model.Entities;
using BPT_Service.Model.Entities.ServiceModel;
using BPT_Service.Model.Enums;
using BPT_Service.Model.Infrastructure.Interfaces;
using Microsoft.AspNetCore.Http;

namespace BPT_Service.Application.ProviderService.Command.RegisterProviderService
{
    public class RegisterProviderServiceCommand : IRegisterProviderServiceCommand
    {
        private readonly IRepository<Provider, Guid> _providerRepository;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public RegisterProviderServiceCommand(IHttpContextAccessor httpContextAccessor,
        IRepository<Provider, Guid> providerRepository)
        {
            _httpContextAccessor = httpContextAccessor;
            _providerRepository = providerRepository;
        }

        public async Task<CommandResult<ProviderServiceViewModel>> ExecuteAsync(ProviderServiceViewModel vm)
        {
            try
            {
                var userId = _httpContextAccessor.HttpContext.User.Identity.Name;
                if (userId == null)
                {
                    return new CommandResult<ProviderServiceViewModel>
                    {
                        isValid = false,
                        myModel = vm
                    };
                }
                var mappingProvider = MappingProvider(vm, Guid.Parse(userId));
                _providerRepository.Add(mappingProvider);
                _providerRepository.SaveAsync();
                vm.Id = mappingProvider.Id.ToString();
                return new CommandResult<ProviderServiceViewModel>
                {
                    isValid = true,
                    myModel = vm
                };
            }
            catch (System.Exception ex)
            {

                return new CommandResult<ProviderServiceViewModel>
                {
                    isValid = false,
                    myModel = vm,
                    errorMessage = ex.InnerException.ToString()
                };
            }
        }
        private Provider MappingProvider(ProviderServiceViewModel vm, Guid userId)
        {
            Provider pro = new Provider();
            pro.PhoneNumber = vm.PhoneNumber;
            pro.Status = Status.Pending;
            pro.CityId = vm.CityId;
            pro.UserId = userId;
            pro.TaxCode = vm.TaxCode;
            pro.Description = vm.Description;
            pro.DateCreated = DateTime.Now;
            pro.ProviderName = vm.ProviderName;
            pro.Address = vm.Address;
            return pro;
        }
    }
}
