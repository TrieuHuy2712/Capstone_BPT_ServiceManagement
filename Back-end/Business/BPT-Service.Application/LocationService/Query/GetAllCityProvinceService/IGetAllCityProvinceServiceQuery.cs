using BPT_Service.Application.LocationService.ViewModel;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace BPT_Service.Application.LocationService.Query.GetAllCityProvinceService
{
    public interface IGetAllCityProvinceServiceQuery
    {
        Task<List<CityProvinceViewModel>> ExecuteAsync();
    }
}
