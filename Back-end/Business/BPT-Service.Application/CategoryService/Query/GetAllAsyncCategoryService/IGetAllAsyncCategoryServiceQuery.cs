using System.Collections.Generic;
using System.Threading.Tasks;
using BPT_Service.Application.CategoryService.ViewModel;

namespace BPT_Service.Application.CategoryService.Query.GetAllAsyncCategoryService
{
    public interface IGetAllAsyncCategoryServiceQuery
    {
        Task<List<CategoryServiceViewModel>> ExecuteAsync();
    }
}