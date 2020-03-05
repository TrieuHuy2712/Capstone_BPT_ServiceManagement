using BPT_Service.Application.LocationService.ViewModel;
using BPT_Service.Model.Entities;
using BPT_Service.Model.Infrastructure.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace BPT_Service.Application.LocationService.Command.DeleteCityProvinceService
{
    public class DeleteCityProvinceServiceCommand : IDeleteCityProvinceServiceCommand
    {
        private readonly IRepository<CityProvince, int> _cityProvinceRepository; 
        public DeleteCityProvinceServiceCommand(IRepository<CityProvince, int> cityProvinceRepository)
        {
            _cityProvinceRepository = cityProvinceRepository;
        }

        public async Task<CommandResult<CityProvinceViewModel>> ExecuteAsync(int id)
        {
            try
            {
                //Check User has permission delete this Role
                var getById = await _cityProvinceRepository.FindByIdAsync(id);
                if (getById == null)
                {
                    return new CommandResult<CityProvinceViewModel>
                    {
                        isValid = false,
                        errorMessage = "Cannot find Id"
                    };
                }
                _cityProvinceRepository.Remove(getById);
                await _cityProvinceRepository.SaveAsync();
                return new CommandResult<CityProvinceViewModel>
                {
                    isValid = true,
                    errorMessage = "Has Delete"
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
    }
}
