using System.Threading.Tasks;
using BPT_Service.Application.CategoryService.ViewModel;
using BPT_Service.Model.Entities;

namespace BPT_Service.Application.CategoryService.Command.DeleteCategoryService
{
    public interface IDeleteCategoryServiceCommand
    {
          Task<CommandResult<CategoryServiceViewModel>> ExecuteAsync(int id);
    }
}