using System;
using System.Linq;
using System.Threading.Tasks;
using BPT_Service.Application.ProviderService.ViewModel;
using BPT_Service.Common.Dtos;
using BPT_Service.Model.Entities.ServiceModel;
using BPT_Service.Model.Infrastructure.Interfaces;

namespace BPT_Service.Application.ProviderService.Query.GetAllPagingProviderService
{
    public class GetAllPagingProviderServiceQuery : IGetAllPagingProviderServiceQuery
    {
        private readonly IRepository<Provider, Guid> _providerRepository;
        public GetAllPagingProviderServiceQuery(IRepository<Provider, Guid> providerRepository)
        {
            _providerRepository = providerRepository;
        }
        public async Task<PagedResult<ProviderServiceViewModel>> ExecuteAsync(string keyword, int page, int pageSize)
        {
            try
            {
                var query = await _providerRepository.FindAllAsync();
                if (!string.IsNullOrEmpty(keyword))
                    query = query.Where(x => x.ProviderName.Contains(keyword)
                    || x.Description.Contains(keyword));

                int totalRow = query.Count();
                query = query.Skip((page - 1) * pageSize)
                   .Take(pageSize);

                var data = query.Select(x => new ProviderServiceViewModel
                {
                    Id = x.Id.ToString(),
                    Address = x.Address,
                    CityId = x.CityId,
                    DateCreated = x.DateCreated,
                    DateModified = x.DateModified,
                    Description = x.Description,
                    LocationCity = new LocationCityViewModel
                    {
                        Id = x.ServiceCityProvince.Id,
                        City = x.ServiceCityProvince.City,
                        Province = x.ServiceCityProvince.Province
                    },
                    PhoneNumber = x.PhoneNumber,
                    ProviderName = x.ProviderName,
                    Status = x.Status,
                    TaxCode = x.TaxCode,
                    UserName = x.AppUser.UserName
                }).ToList();

                var paginationSet = new PagedResult<ProviderServiceViewModel>()
                {
                    Results = data,
                    CurrentPage = page,
                    RowCount = totalRow,
                    PageSize = pageSize
                };

                return paginationSet;
            }
            catch (System.Exception)
            {

                return new PagedResult<ProviderServiceViewModel>()
                {
                    Results = null,
                    CurrentPage = page,
                    RowCount = 0,
                    PageSize = pageSize
                };
            }
        }
    }
}