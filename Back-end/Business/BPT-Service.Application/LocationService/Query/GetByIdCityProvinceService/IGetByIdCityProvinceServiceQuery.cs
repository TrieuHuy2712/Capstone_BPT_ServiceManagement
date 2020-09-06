using BPT_Service.Application.LocationService.ViewModel;
using BPT_Service.Model.Entities;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace BPT_Service.Application.LocationService.Query.GetByIdCityProvinceService
{
    public interface IGetByIdCityProvinceServiceQuery
    {
        Task<CommandResult<CityProvinceViewModel>> ExecuteAsync(int id);
    }
}
