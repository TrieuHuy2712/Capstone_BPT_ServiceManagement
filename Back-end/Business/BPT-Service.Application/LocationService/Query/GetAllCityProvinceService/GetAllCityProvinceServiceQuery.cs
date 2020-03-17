using BPT_Service.Application.LocationService.ViewModel;
using BPT_Service.Model.Entities;
using BPT_Service.Model.Infrastructure.Interfaces;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BPT_Service.Application.LocationService.Query.GetAllCityProvinceService
{
    public class GetAllCityProvinceServiceQuery : IGetAllCityProvinceServiceQuery
    {
        private readonly IRepository<CityProvince, int> _cityProvinceRepository;
        public GetAllCityProvinceServiceQuery(IRepository<CityProvince, int> cityProvinceRepository)
        {
            _cityProvinceRepository = cityProvinceRepository;
        }

        public async Task<List<CityProvinceViewModel>> ExecuteAsync()
        {
            var getAll = await _cityProvinceRepository.FindAllAsync();
            return getAll.Select(x => new CityProvinceViewModel
            {
                Id = x.Id,
                City = x.City,
                Province = x.Province
            }).ToList();
        }
    }
}
