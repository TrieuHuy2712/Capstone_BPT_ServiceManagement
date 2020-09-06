using BPT_Service.Application.LocationService.ViewModel;
using BPT_Service.Common.Dtos;
using BPT_Service.Model.Entities;
using BPT_Service.Model.Infrastructure.Interfaces;
using System.Linq;
using System.Threading.Tasks;

namespace BPT_Service.Application.LocationService.Query.GetAllPagingCityProvinceService
{
    public class GetAllPagingCityProvinceServiceQuery : IGetAllPagingCityProvinceServiceQuery
    {
        private readonly IRepository<CityProvince, int> _cityProvinceRepository;

        public GetAllPagingCityProvinceServiceQuery(IRepository<CityProvince, int> cityProvinceRepository)
        {
            _cityProvinceRepository = cityProvinceRepository;
        }

        public async Task<PagedResult<CityProvinceViewModel>> ExecuteAsync(string keyword, int page, int pageSize)
        {
            var query = await _cityProvinceRepository.FindAllAsync();
            if (!string.IsNullOrEmpty(keyword))
                query = query.Where(x => x.Province.Contains(keyword)
                || x.City.Contains(keyword));

            int totalRow = query.Count();
            if (pageSize != 0)
            {
                query = query.Skip((page - 1) * pageSize).Take(pageSize);
            }

            var data = query.Select(x => new CityProvinceViewModel
            {
                Id = x.Id,
                City = x.City,
                Province = x.Province,
                ImgPath = x.ImgPath
            }).ToList();

            var paginationSet = new PagedResult<CityProvinceViewModel>()
            {
                Results = data,
                CurrentPage = page,
                RowCount = totalRow,
                PageSize = pageSize
            };

            return paginationSet;
        }
    }
}