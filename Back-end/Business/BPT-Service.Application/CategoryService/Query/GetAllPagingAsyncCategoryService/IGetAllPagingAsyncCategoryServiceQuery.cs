using System.Threading.Tasks;
using BPT_Service.Application.CategoryService.ViewModel;
using BPT_Service.Common.Dtos;

namespace BPT_Service.Application.CategoryService.Query.GetAllPagingAsyncCategoryService
{
    public interface IGetAllPagingAsyncCategoryServiceQuery
    {
        Task<PagedResult<CategoryServiceViewModel>> ExecuteAsync(string keyword, int page, int pageSize);
    }
}