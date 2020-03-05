using System.Threading.Tasks;
using BPT_Service.Application.NewsProviderService.ViewModel;
using BPT_Service.Common.Dtos;

namespace BPT_Service.Application.NewsProviderService.Query.GetAllPagingProviderNewsOfProvider
{
    public interface IGetAllPagingProviderNewsOfProviderQuery
    {
         Task<PagedResult<NewsProviderViewModel>> ExecuteAsync(string keyword, int page, int pageSize);
    }
}