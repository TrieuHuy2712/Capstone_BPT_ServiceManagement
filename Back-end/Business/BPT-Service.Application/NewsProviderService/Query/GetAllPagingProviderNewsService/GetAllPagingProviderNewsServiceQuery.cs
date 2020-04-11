using BPT_Service.Application.NewsProviderService.ViewModel;
using BPT_Service.Application.ProviderService.Query.GetByIdProviderService;
using BPT_Service.Common.Dtos;
using BPT_Service.Common.Support;
using BPT_Service.Model.Entities.ServiceModel.ProviderServiceModel;
using BPT_Service.Model.Infrastructure.Interfaces;
using Microsoft.AspNetCore.Http;
using System.Linq;
using System.Threading.Tasks;

namespace BPT_Service.Application.NewsProviderService.Query.GetAllPagingProviderNewsService
{
    public class GetAllPagingProviderNewsServiceQuery : IGetAllPagingProviderNewsServiceQuery
    {
        private readonly IRepository<ProviderNew, int> _providerNewRepository;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IGetByIdProviderServiceQuery _getByIdProviderServiceQuery;

        public GetAllPagingProviderNewsServiceQuery(
            IRepository<ProviderNew, int> providerNewRepository, 
            IHttpContextAccessor httpContextAccessor, 
            IGetByIdProviderServiceQuery getByIdProviderServiceQuery)
        {
            _providerNewRepository = providerNewRepository;
            _httpContextAccessor = httpContextAccessor;
            _getByIdProviderServiceQuery = getByIdProviderServiceQuery;
        }

        public async Task<PagedResult<NewsProviderViewModel>> ExecuteAsync(string keyword, int page, int pageSize, bool isAdminPage, int filter)
        {
            try
            {
                var query = await _providerNewRepository.FindAllAsync();
                if (!string.IsNullOrEmpty(keyword))
                    query = query.Where(x => x.Author.ToLower().Contains(keyword.ToLower())
                    || LevenshteinDistance.Compute(x.Author.ToLower(), keyword.ToLower()) <= 3
                    || x.Content.ToLower().Contains(keyword.ToLower())
                    || LevenshteinDistance.Compute(x.Content.ToLower(), keyword.ToLower()) <= 3
                    || x.Title.ToLower().Contains(keyword.ToLower())
                    || LevenshteinDistance.Compute(x.Title.ToLower(), keyword.ToLower()) <= 3);

                int totalRow = query.Count();
                if (pageSize != 0)
                {
                    query = query.Skip((page - 1) * pageSize)
                       .Take(pageSize);
                }

                var data = query.Select(x => new NewsProviderViewModel
                {
                    Id = x.Id,
                    ImgPath = x.ImgPath,
                    Author = x.Author,
                    Content = x.Content,
                    DateCreated = x.DateCreated,
                    DateModified = x.DateModified == null ? x.DateCreated : x.DateModified,
                    ProviderId = x.ProviderId.ToString(),
                    Title = x.Title,
                    Status = x.Status,
                    Reason = "",
                    ProviderName = _getByIdProviderServiceQuery.ExecuteAsync(x.ProviderId.ToString()).Result.myModel.ProviderName
                }).ToList();
                int filtering = filter;
                switch (filtering)
                {
                    case 1:
                        data = data.Where(x => x.Status == Model.Enums.Status.Active).ToList();
                        break;
                    case 0:
                        data = data.Where(x => x.Status == Model.Enums.Status.InActive).ToList();
                        break;
                    case 2:
                        data = data.Where(x => x.Status == Model.Enums.Status.Pending).ToList();
                        break;
                    default:
                        data = data;
                        break;
                }
                if (isAdminPage)
                {
                    var paginationSet = new PagedResult<NewsProviderViewModel>()
                    {
                        Results = data,
                        CurrentPage = page,
                        RowCount = totalRow,
                        PageSize = pageSize
                    };
                    return paginationSet;
                }
                else
                {
                    var paginationSet = new PagedResult<NewsProviderViewModel>()
                    {
                        Results = data.Where(x => x.Status == Model.Enums.Status.Active).ToList(),
                        CurrentPage = page,
                        RowCount = totalRow,
                        PageSize = pageSize
                    };
                    return paginationSet;
                }
            }
            catch (System.Exception)
            {
                return new PagedResult<NewsProviderViewModel>()
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