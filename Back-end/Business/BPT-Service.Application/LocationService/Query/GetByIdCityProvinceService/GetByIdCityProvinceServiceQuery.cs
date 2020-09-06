using BPT_Service.Application.LocationService.ViewModel;
using BPT_Service.Model.Entities;
using BPT_Service.Model.Infrastructure.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace BPT_Service.Application.LocationService.Query.GetByIdCityProvinceService
{
    public class GetByIdCityProvinceServiceQuery : IGetByIdCityProvinceServiceQuery
    {
        private readonly IRepository<CityProvince, int> _cityProvinceRepository;
        public GetByIdCityProvinceServiceQuery(IRepository<CityProvince, int> cityProvinceRepository)
        {
            _cityProvinceRepository = cityProvinceRepository;
        }

        public async Task<CommandResult<CityProvinceViewModel>> ExecuteAsync(int id)
        {
            try
            {
                var findById = await _cityProvinceRepository.FindByIdAsync(id);
                if (findById == null)
                {
                    return new CommandResult<CityProvinceViewModel>
                    {
                        isValid = false,
                        errorMessage = "Cannot find your id",
                    };
                }
                return new CommandResult<CityProvinceViewModel>
                {
                    isValid = true,
                    myModel = new CityProvinceViewModel
                    {
                        Id = findById.Id,
                        City = findById.City,
                        Province = findById.Province
                    }
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
