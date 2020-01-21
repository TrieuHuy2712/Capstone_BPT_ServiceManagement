using System.Threading.Tasks;
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
        public async Task<bool> ExecuteAsync(int id)
        {
            var CategoryDel = await _categoryRepository.FindByIdAsync(id);
            if (CategoryDel != null)
            {
                _categoryRepository.Remove(CategoryDel);
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}