using System.Threading.Tasks;
using BPT_Service.Application.CategoryService.ViewModel;
using BPT_Service.Model.Entities;

namespace BPT_Service.Application.CategoryService.Command.UpdateCategoryService
{
    public interface IUpdateCategoryServiceCommand
    {
         Task<CommandResult<CategoryServiceViewModel>> ExecuteAsync(CategoryServiceViewModel userVm);
    }
}