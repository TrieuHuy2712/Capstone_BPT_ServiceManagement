using System;
using System.Security.Claims;
using System.Threading.Tasks;
using BPT_Service.Application.ProviderService.ViewModel;
using BPT_Service.Model.Entities;
using BPT_Service.Model.Entities.ServiceModel;
using BPT_Service.Model.Infrastructure.Interfaces;
using Microsoft.AspNetCore.Http;

namespace BPT_Service.Application.ProviderService.Query.GetByIdProviderService
{
    public class GetByIdProviderServiceQuery : IGetByIdProviderServiceQuery
    {
        private readonly IRepository<Provider, Guid> _providerRepository;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public GetByIdProviderServiceQuery(IRepository<Provider, Guid> providerRepository,
        IHttpContextAccessor httpContextAccessor)
        {
            _providerRepository = providerRepository;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<CommandResult<ProviderServiceViewModel>> ExecuteAsync(string id)
        {
            try
            {
                var userId = _httpContextAccessor.HttpContext.User.Identity.Name;
                var getPro = await _providerRepository.FindByIdAsync(Guid.Parse(id));
                if (getPro == null)
                {
                    return new CommandResult<ProviderServiceViewModel>
                    {
                        isValid = false,
                        myModel = null
                    };
                }
                if (getPro.AppUser.Id != Guid.Parse(userId))
                {
                    return new CommandResult<ProviderServiceViewModel>
                    {
                        isValid = false,
                        myModel = null
                    };
                }
                var mappingProvider = MappingProvider(getPro);
                return new CommandResult<ProviderServiceViewModel>
                {
                    isValid = true,
                    myModel = mappingProvider
                };
            }
            catch (Exception ex)
            {
                return new CommandResult<ProviderServiceViewModel>
                {
                    isValid = false,
                    errorMessage = ex.InnerException.ToString()
                };
            }
        }

        private ProviderServiceViewModel MappingProvider(Provider vm)
        {
            ProviderServiceViewModel pro = new ProviderServiceViewModel();
            pro.Id = vm.Id.ToString();
            pro.PhoneNumber = vm.PhoneNumber;
            pro.Status = vm.Status;
            pro.CityId = vm.CityId;
            pro.UserId = vm.UserId.ToString();
            pro.TaxCode = pro.TaxCode;
            pro.Description = pro.Description;
            pro.DateModified = DateTime.Now;
            pro.ProviderName = vm.ProviderName;
            pro.Address = vm.Address;
            pro.LocationCity.City = vm.ServiceCityProvince.City;
            pro.LocationCity.Province = vm.ServiceCityProvince.Province;
            return pro;
        }
    }
}