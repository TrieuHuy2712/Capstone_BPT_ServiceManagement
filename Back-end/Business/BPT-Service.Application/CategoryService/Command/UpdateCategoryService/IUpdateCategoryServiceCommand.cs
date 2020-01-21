using System.Threading.Tasks;
using BPT_Service.Application.CategoryService.ViewModel;

namespace BPT_Service.Application.CategoryService.Command.UpdateCategoryService
{
    public interface IUpdateCategoryServiceCommand
    {
         Task<bool> ExecuteAsync(CategoryServiceViewModel userVm);
    }
}