using System.Threading.Tasks;
using BPT_Service.Application.CategoryService.ViewModel;

namespace BPT_Service.Application.CategoryService.Command.AddCategoryService
{
    public interface IAddCategoryServiceCommand
    {
        Task<bool> ExecuteAsync(CategoryServiceViewModel userVm);
    }
}