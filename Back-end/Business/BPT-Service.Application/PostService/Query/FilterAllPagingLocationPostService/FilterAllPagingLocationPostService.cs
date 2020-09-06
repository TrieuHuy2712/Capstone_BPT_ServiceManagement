using BPT_Service.Application.PostService.Query.FilterAllPagingPostService;
using BPT_Service.Application.PostService.ViewModel;
using BPT_Service.Common.Constants;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BPT_Service.Application.PostService.Query.FilterAllPagingLocationPostService
{
    public class FilterAllPagingLocationPostService : IFilterAllPagingLocationPostService
    {
        private readonly IFilterAllPagingPostServiceQuery _filterAllPagingPostServiceQuery;

        public FilterAllPagingLocationPostService(IFilterAllPagingPostServiceQuery filterAllPagingPostServiceQuery)
        {
            _filterAllPagingPostServiceQuery = filterAllPagingPostServiceQuery;
        }

        public async Task<List<ListLocationPostViewModel>> ExecuteAsync(int typeCategory, int page, int pageSize, string nameLocation)
        {
            var getAllByLocation = await _filterAllPagingPostServiceQuery.
                ExecuteAsync(1, 0, KindOfDetailService.FILTER_BY_LOCATION, nameLocation);

            var groupByCategory = getAllByLocation.Results.GroupBy(x => new
            {
                x.CategoryId,
                x.CategoryName
            }).Select(x => new ListLocationPostViewModel
            {
                CategoryName = x.Key.CategoryName,
                CategoryId = x.Key.CategoryId,
                ListService = pageSize==0 ? x.ToList() : x.Skip((page - 1) * pageSize).Take(pageSize).ToList()
            }).ToList();

            if (typeCategory != 0)
            {
                groupByCategory.Where(x => x.CategoryId == typeCategory);
            }
            return groupByCategory;
        }
    }
}