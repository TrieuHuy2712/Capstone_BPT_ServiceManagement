using BPT_Service.Application.ProviderService.ViewModel;
using BPT_Service.Common.Dtos;
using BPT_Service.Common.Support;
using BPT_Service.Model.Entities;
using BPT_Service.Model.Entities.ServiceModel;
using BPT_Service.Model.Infrastructure.Interfaces;
using Microsoft.AspNetCore.Identity;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace BPT_Service.Application.ProviderService.Query.GetAllPagingProviderService
{
    public class GetAllPagingProviderServiceQuery : IGetAllPagingProviderServiceQuery
    {
        private readonly IRepository<Provider, Guid> _providerRepository;
        private readonly IRepository<CityProvince, int> _cityRepository;
        private readonly UserManager<AppUser> _userManager;

        public GetAllPagingProviderServiceQuery(IRepository<Provider, Guid> providerRepository,
            IRepository<CityProvince, int> cityRepository,
            UserManager<AppUser> userManager)
        {
            _providerRepository = providerRepository;
            _cityRepository = cityRepository;
            _userManager = userManager;
        }

        public async Task<PagedResult<ProviderServiceViewModel>> ExecuteAsync(string keyword, int page, int pageSize, int filter)
        {
            try
            {
                var query = await _providerRepository.FindAllAsync();
                if (!string.IsNullOrEmpty(keyword))
                    query = query.Where(x => x.ProviderName.ToLower().Contains(keyword.ToLower())
                    //|| _levenshteinDistance.Compute(x.ProviderName.ToLower(), keyword.ToLower()) <= 3
                    || x.Description.ToLower().Contains(keyword.ToLower())
                    //|| _levenshteinDistance.Compute(x.Description.ToLower(), keyword.ToLower()) <= 3
                     || x.TaxCode.ToLower().Contains(keyword.ToLower())
                    //|| _levenshteinDistance.Compute(x.TaxCode.ToLower(), keyword.ToLower()) <= 3
                     || x.PhoneNumber.ToLower().Contains(keyword.ToLower())
                    //|| _levenshteinDistance.Compute(x.PhoneNumber.ToLower(), keyword.ToLower()) <= 3
                    && x.Status == Model.Enums.Status.Active);

                int totalRow = query.Count();
                if (pageSize != 0)
                {
                    query = query.Skip((page - 1) * pageSize)
                  .Take(pageSize);
                }
               
                var getAllLocation = await _cityRepository.FindAllAsync();
                var data = query.Select(x => new ProviderServiceViewModel
                {
                    Id = x.Id.ToString(),
                    Address = x.Address,
                    CityId = x.CityId,
                    DateCreated = x.DateCreated,
                    DateModified = x.DateModified,
                    Description = x.Description,
                    PhoneNumber = x.PhoneNumber,
                    ProviderName = x.ProviderName,
                    Status = x.Status,
                    TaxCode = x.TaxCode,
                    UserId = x.UserId.ToString(),
                    AvatarPath=x.AvartarPath,
                }).ToList();

                var map = (from loc in getAllLocation
                           join x in data
                           on loc.Id equals x.CityId
                           select new ProviderServiceViewModel
                           {
                               Id = x.Id.ToString(),
                               Address = x.Address,
                               CityId = x.CityId,
                               DateCreated = x.DateCreated,
                               DateModified = x.DateModified,
                               Description = x.Description,
                               PhoneNumber = x.PhoneNumber,
                               ProviderName = x.ProviderName,
                               Status = x.Status,
                               TaxCode = x.TaxCode,
                               CityName = loc.City+"_"+loc.Province,
                               ProvinceName = loc.Province,
                               UserId = x.UserId,
                               AvatarPath = x.AvatarPath,
                               UserName = _userManager.FindByIdAsync(x.UserId).Result.UserName,
                               Reason = ""
                           }).ToList();
                int filtering = filter;
                switch (filtering)
                {
                    case 1:
                        map = map.Where(x => x.Status == Model.Enums.Status.Active).ToList();
                        break;
                    case 0:
                        map = map.Where(x => x.Status == Model.Enums.Status.InActive).ToList();
                        break;
                    case 2:
                        map = map.Where(x => x.Status == Model.Enums.Status.Pending).ToList();
                        break;
                    default:
                        map=map;
                        break;
                }

                var paginationSet = new PagedResult<ProviderServiceViewModel>()
                {
                    Results = map,
                    CurrentPage = page,
                    RowCount = totalRow,
                    PageSize = pageSize
                };

                return paginationSet;
            }
            catch (System.Exception ex)
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