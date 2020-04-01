using BPT_Service.Application.ProviderService.ViewModel;
using BPT_Service.Model.Entities;
using BPT_Service.Model.Entities.ServiceModel;
using BPT_Service.Model.Infrastructure.Interfaces;
using Microsoft.AspNetCore.Http;
using System;
using System.Threading.Tasks;

namespace BPT_Service.Application.ProviderService.Query.GetByIdProviderService
{
    public class GetByIdProviderServiceQuery : IGetByIdProviderServiceQuery
    {
        private readonly IRepository<Provider, Guid> _providerRepository;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IRepository<CityProvince, int> _locationRepository;

        public GetByIdProviderServiceQuery(IRepository<Provider, Guid> providerRepository,
        IHttpContextAccessor httpContextAccessor,
        IRepository<CityProvince, int> locationRepository)
        {
            _providerRepository = providerRepository;
            _httpContextAccessor = httpContextAccessor;
            _locationRepository = locationRepository;
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
                //Check don't have location
                var location = await _locationRepository.FindSingleAsync(x => x.Id == getPro.CityId);
                if (location == null)
                {
                    return new CommandResult<ProviderServiceViewModel>
                    {
                        isValid = false,
                        errorMessage = "Cannot find your location"
                    };
                }
                if (getPro.UserId != Guid.Parse(userId))
                {
                    return new CommandResult<ProviderServiceViewModel>
                    {
                        isValid = false,
                        myModel = null
                    };
                }
                var mappingProvider = MappingProvider(getPro, location);
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

        private ProviderServiceViewModel MappingProvider(Provider vm, CityProvince cityProvince)
        {
            ProviderServiceViewModel pro =  new ProviderServiceViewModel();
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
            pro.CityName = cityProvince.City;
            pro.ProvinceName = cityProvince.Province;
            pro.AvatarPath = vm.AvartarPath;
            return pro;
        }
    }
}