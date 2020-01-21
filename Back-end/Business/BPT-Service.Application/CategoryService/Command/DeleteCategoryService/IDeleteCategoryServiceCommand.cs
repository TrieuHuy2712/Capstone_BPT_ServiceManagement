using System.Threading.Tasks;
using BPT_Service.Application.CategoryService.ViewModel;

namespace BPT_Service.Application.CategoryService.Command.DeleteCategoryService
{
    public interface IDeleteCategoryServiceCommand
    {
          Task<bool> ExecuteAsync(int id);
    }
}