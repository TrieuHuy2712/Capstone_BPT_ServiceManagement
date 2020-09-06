using BPT_Service.Application.PostService.ViewModel;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BPT_Service.Application.PostService.Query.FilterAllPagingLocationPostService
{
    public interface IFilterAllPagingLocationPostService
    {
        public Task<List<ListLocationPostViewModel>> ExecuteAsync(int typeCategory, int pageIndex, int pageSize, string nameLocation);
    }
}