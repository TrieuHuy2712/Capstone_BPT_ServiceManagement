using System;
using System.Threading.Tasks;
using BPT_Service.Application.ProviderService.ViewModel;
using BPT_Service.Model.Entities;
using BPT_Service.Model.Entities.ServiceModel;
using BPT_Service.Model.Infrastructure.Interfaces;

namespace BPT_Service.Application.ProviderService.Command.UpdateProviderService
{
    public class UpdateProviderServiceCommand : IUpdateProviderServiceCommand
    {
        private readonly IRepository<Provider, Guid> _providerRepostiroy;
        public UpdateProviderServiceCommand(IRepository<Provider, Guid> providerRepostiroy)
        {
            _providerRepostiroy = providerRepostiroy;
        }
        public async Task<CommandResult<ProviderServiceViewModel>> ExecuteAsync(ProviderServiceViewModel vm)
        {
            try
            {
                var getProviderService = await _providerRepostiroy.FindByIdAsync(Guid.Parse(vm.Id));
                if (getProviderService != null)
                {
                    var mappingProvider = MappingProvider(getProviderService, vm);
                    _providerRepostiroy.Update(mappingProvider);
                    await _providerRepostiroy.SaveAsync();
                    return new CommandResult<ProviderServiceViewModel>
                    {
                        isValid = true,
                        myModel = vm
                    };
                }
                return new CommandResult<ProviderServiceViewModel>
                {
                    isValid = false,
                    errorMessage = "Cannot not find Id of Provider"
                };
            }
            catch (System.Exception ex)
            {
                return new CommandResult<ProviderServiceViewModel>
                {
                    isValid = false,
                    errorMessage = ex.InnerException.ToString()
                };
            }
        }

        private Provider MappingProvider(Provider pro, ProviderServiceViewModel vm)
        {
            pro.PhoneNumber = vm.PhoneNumber;
            pro.Status = vm.Status;
            pro.CityId = vm.CityId;
            pro.TaxCode = vm.TaxCode;
            pro.Description = vm.Description;
            pro.DateModified = DateTime.Now;
            pro.ProviderName = vm.ProviderName;
            pro.Address = vm.Address;
            return pro;
        }
    }
}