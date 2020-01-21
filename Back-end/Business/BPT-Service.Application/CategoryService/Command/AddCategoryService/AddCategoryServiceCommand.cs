using System.Threading.Tasks;
using BPT_Service.Application.CategoryService.ViewModel;
using BPT_Service.Model.Entities;
using BPT_Service.Model.Infrastructure.Interfaces;

namespace BPT_Service.Application.CategoryService.Command.AddCategoryService
{
    public class AddCategoryServiceCommand : IAddCategoryServiceCommand
    {
        private readonly IRepository<Category, int> _categoryRepository;
        public AddCategoryServiceCommand(IRepository<Category, int> categoryRepository, IUnitOfWork unitOfWork)
        {
            _categoryRepository = categoryRepository;
        }
        public async Task<bool> ExecuteAsync(CategoryServiceViewModel userVm)
        {
            Category category = new Category();
            category.Id = userVm.Id;
            category.CategoryName = userVm.CategoryName;
            category.NameVietnamese = userVm.NameVietnamese;
            category.Description = userVm.Description;
            _categoryRepository.Add(category);
            return true;
        }
    }
}