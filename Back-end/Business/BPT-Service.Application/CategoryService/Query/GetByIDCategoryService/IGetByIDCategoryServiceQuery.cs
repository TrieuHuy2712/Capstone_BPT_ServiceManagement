using System.Threading.Tasks;
using BPT_Service.Application.CategoryService.ViewModel;

namespace BPT_Service.Application.CategoryService.Query.GetByIDCategoryService
{
    public interface IGetByIDCategoryServiceQuery
    {
        Task<CategoryServiceViewModel> ExecuteAsync(int id);
    }
}