using System.Linq;
using System.Threading.Tasks;
using BPT_Service.Application.NewsProviderService.ViewModel;
using BPT_Service.Common.Dtos;
using BPT_Service.Model.Entities.ServiceModel.ProviderServiceModel;
using BPT_Service.Model.Infrastructure.Interfaces;
using Microsoft.AspNetCore.Http;

namespace BPT_Service.Application.NewsProviderService.Query.GetAllPagingProviderNewsService
{
    public class GetAllPagingProviderNewsServiceQuery : IGetAllPagingProviderNewsServiceQuery
    {
        private readonly IRepository<ProviderNew, int> _providerNewRepository;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public GetAllPagingProviderNewsServiceQuery(IRepository<ProviderNew, int> providerNewRepository,
        IHttpContextAccessor httpContextAccessor)
        {
            _providerNewRepository = providerNewRepository;
            _httpContextAccessor = httpContextAccessor;
        }
        public async Task<PagedResult<NewsProviderViewModel>> ExecuteAsync(string keyword, int page, int pageSize)
        {
            try
            {
                var query = await _providerNewRepository.FindAllAsync();
                if (!string.IsNullOrEmpty(keyword))
                    query = query.Where(x => x.Author.Contains(keyword)
                    || x.Content.Contains(keyword) || x.Title.Contains(keyword));

                int totalRow = query.Count();
                query = query.Skip((page - 1) * pageSize)
                   .Take(pageSize);

                var data = query.Select(x => new NewsProviderViewModel
                {
                    Author = x.Author,
                    Content = x.Content,
                    Status = x.Status,
                    ProviderName = x.Provider.ProviderName,
                    Title = x.Title,
                    Id = x.Id,
                }).ToList();

                var paginationSet = new PagedResult<NewsProviderViewModel>()
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