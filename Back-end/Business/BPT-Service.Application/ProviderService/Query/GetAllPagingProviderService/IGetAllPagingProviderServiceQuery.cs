using System.Threading.Tasks;
using BPT_Service.Application.ProviderService.ViewModel;
using BPT_Service.Common.Dtos;

namespace BPT_Service.Application.ProviderService.Query.GetAllPagingProviderService
{
    public interface IGetAllPagingProviderServiceQuery
    {
         Task<PagedResult<ProviderServiceViewModel>> ExecuteAsync(string keyword, int page, int pageSize, int filter);
    }
}