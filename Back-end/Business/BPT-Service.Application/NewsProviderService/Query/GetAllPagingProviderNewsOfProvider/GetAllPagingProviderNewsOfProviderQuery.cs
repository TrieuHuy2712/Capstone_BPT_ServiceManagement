using System;
using System.Linq;
using System.Threading.Tasks;
using BPT_Service.Application.NewsProviderService.ViewModel;
using BPT_Service.Common.Dtos;
using BPT_Service.Model.Entities;
using BPT_Service.Model.Entities.ServiceModel;
using BPT_Service.Model.Entities.ServiceModel.ProviderServiceModel;
using BPT_Service.Model.Infrastructure.Interfaces;
using Microsoft.AspNetCore.Http;

namespace BPT_Service.Application.NewsProviderService.Query.GetAllPagingProviderNewsOfProvider
{
    public class GetAllPagingProviderNewsOfProviderQuery : IGetAllPagingProviderNewsOfProviderQuery
    {
        private readonly IRepository<ProviderNew, int> _providerNewRepository;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IRepository<Provider, Guid> _providerRepository;
        public GetAllPagingProviderNewsOfProviderQuery(IRepository<ProviderNew, int> providerNewRepository,
        IHttpContextAccessor httpContextAccessor,
        IRepository<Provider, Guid> providerRepository)
        {
            _providerNewRepository = providerNewRepository;
            _httpContextAccessor = httpContextAccessor;
            _providerRepository = providerRepository;
        }
        public async Task<PagedResult<NewsProviderViewModel>> ExecuteAsync(string keyword, int page, int pageSize)
        {
            try
            {
                var getProviderId = _httpContextAccessor.HttpContext.User.Identity.Name;
                if (getProviderId == null)
                {
                    return new PagedResult<NewsProviderViewModel>()
                    {
                        Results = null,
                        CurrentPage = page,
                        RowCount = 0,
                        PageSize = pageSize
                    };
                }
                //var query = await _providerNewRepository.FindAllAsync(x => x.Provider.AppUser.UserName == getProviderId);
                var getAllProvider = await _providerRepository.FindAllAsync();
                var getAllProviderNew = await _providerNewRepository.FindAllAsync();

                var query = (from provider in getAllProvider.ToList()
                             join news in getAllProviderNew.ToList()
                             on provider.Id equals news.ProviderId
                             where provider.UserId == Guid.Parse(getProviderId)
                             select new NewsProviderViewModel
                             {
                                 Id = news.Id,
                                 ImgPath = news.ImgPath,
                                 Author = news.Author,
                                 Content = news.Content,
                                 DateCreated = news.DateCreated,
                                 DateModified = news.DateModified == null ? news.DateCreated : news.DateModified,
                                 ProviderId = provider.Id.ToString(),
                                 ProviderName = provider.ProviderName,
                                 Status = provider.Status,
                                 Title = news.Title
                             });

                if (!string.IsNullOrEmpty(keyword))
                    query = query.Where(x => x.Author.Contains(keyword)
                    || x.Content.Contains(keyword) || x.Title.Contains(keyword));

                int totalRow = query.Count();
                query = query.Skip((page - 1) * pageSize)
                   .Take(pageSize);

                var paginationSet = new PagedResult<NewsProviderViewModel>()
                {
                    Results = query.ToList(),
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