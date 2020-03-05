using BPT_Service.Application.LocationService.ViewModel;
using BPT_Service.Model.Entities;
using BPT_Service.Model.Infrastructure.Interfaces;
using System;
using System.Threading.Tasks;

namespace BPT_Service.Application.LocationService.Command.UpdateCityProvinceService
{
    public class UpdateCityProvinceServiceCommand : IUpdateCityProvinceServiceCommand
    {
        private readonly IRepository<CityProvince, int> _cityRepository;

        public UpdateCityProvinceServiceCommand(IRepository<CityProvince, int> cityRepository)
        {
            _cityRepository = cityRepository;
        }

        public async Task<CommandResult<CityProvinceViewModel>> ExecuteAsync(CityProvinceViewModel vm)
        {
            //Check user has permission
            try
            {
                var getId = await _cityRepository.FindByIdAsync(vm.Id);
                if (getId == null)
                {
                    return new CommandResult<CityProvinceViewModel>
                    {
                        isValid = false,
                        errorMessage = "Cannot find your id"
                    };
                }
                MappingCityProvince(vm, getId);
                _cityRepository.Update(getId);
                await _cityRepository.SaveAsync();
                return new CommandResult<CityProvinceViewModel>
                {
                    isValid = true,
                    myModel = vm
                };
            }
            catch (Exception ex)
            {
                return new CommandResult<CityProvinceViewModel>
                {
                    isValid = false,
                    errorMessage = ex.InnerException.Message.ToString()
                };
            }
        }

        private void MappingCityProvince(CityProvinceViewModel cityProvinceVM, CityProvince cityProvince)
        {
            cityProvinceVM.City = cityProvince.City;
            cityProvinceVM.Province = cityProvince.Province;
        }
    }
}