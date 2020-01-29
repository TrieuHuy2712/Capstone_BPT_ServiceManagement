using System.Threading.Tasks;
using BPT_Service.Application.CategoryService.ViewModel;
using BPT_Service.Model.Entities;
using BPT_Service.Model.Infrastructure.Interfaces;

namespace BPT_Service.Application.CategoryService.Command.DeleteCategoryService
{
    public class DeleteCategoryServiceCommand : IDeleteCategoryServiceCommand
    {
        private readonly IRepository<Category, int> _categoryRepository;
        public DeleteCategoryServiceCommand(IRepository<Category, int> categoryRepository, IUnitOfWork unitOfWork)
        {
            _categoryRepository = categoryRepository;
        }
        public async Task<CommandResult<CategoryServiceViewModel>> ExecuteAsync(int id)
        {
            try
            {
                var CategoryDel = await _categoryRepository.FindByIdAsync(id);
                if (CategoryDel != null)
                {
                    _categoryRepository.Remove(CategoryDel);
                    await _categoryRepository.SaveAsync();
                    return new CommandResult<CategoryServiceViewModel>
                    {
                        isValid = true,
                        myModel = new CategoryServiceViewModel
                        {
                            CategoryName = CategoryDel.CategoryName,
                            Description = CategoryDel.Description,
                            Id = CategoryDel.Id
                        }
                    };
                }
                else
                {
                    return new CommandResult<CategoryServiceViewModel>
                    {
                        isValid = false,
                        errorMessage = "Cannot find your Id"
                    };
                }
            }
            catch (System.Exception ex)
            {
                return new CommandResult<CategoryServiceViewModel>
                {
                    isValid = false,
                    errorMessage = ex.InnerException.ToString()
                };
            }
        }
    }
}