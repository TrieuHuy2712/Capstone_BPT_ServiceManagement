using BPT_Service.Application.LocationService.ViewModel;
using BPT_Service.Common.Dtos;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace BPT_Service.Application.LocationService.Query.GetAllPagingCityProvinceService
{
    public interface IGetAllPagingCityProvinceServiceQuery
    {
        Task<PagedResult<CityProvinceViewModel>> ExecuteAsync(string keyword, int page, int pageSize);
    }
}
