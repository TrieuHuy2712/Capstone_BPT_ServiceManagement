using BPT_Service.Application.LocationService.ViewModel;
using BPT_Service.Model.Entities;
using BPT_Service.Model.Infrastructure.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BPT_Service.Application.LocationService.Command.AddCityProvinceService
{
    public class AddCityProvinceServiceCommand : IAddCityProvinceServiceCommand
    {
        private readonly IRepository<CityProvince, int> _cityProvinceRepository; 
        public AddCityProvinceServiceCommand(IRepository<CityProvince, int> cityProvinceRepository)
        {
            _cityProvinceRepository = cityProvinceRepository;
        }

        public async Task<CommandResult<List<CityProvinceViewModel>>> ExecuteAsync(List<CityProvinceViewModel> listAddViewModel)
        {

            try
            {
                //Check User Has Permission Create
                var listModel = MappingCityProvince(listAddViewModel);
                await _cityProvinceRepository.Add(listModel);
                await _cityProvinceRepository.SaveAsync();
                return new CommandResult<List<CityProvinceViewModel>>
                {
                    isValid = true,
                    myModel = listModel.Select(x => new CityProvinceViewModel {
                        Id = x.Id,
                        City = x.City,
                        Province = x.Province
                    }).ToList()
                };
            }
            catch (Exception ex)
            {
                return new CommandResult<List<CityProvinceViewModel>>
                {
                    isValid = false,
                    errorMessage = ex.InnerException.Message.ToString()
                };
            }
        }
        private List<CityProvince> MappingCityProvince(List<CityProvinceViewModel> listAddViewModel)
        {
            return listAddViewModel.Select(x => new CityProvince
            {
                Id = x.Id,
                City = x.City,
                Province = x.Province
            }).ToList();
        }
    }
}
