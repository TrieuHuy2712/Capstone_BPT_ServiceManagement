using System.Threading.Tasks;
using BPT_Service.Application.CategoryService.ViewModel;
using BPT_Service.Model.Entities;
using BPT_Service.Model.Infrastructure.Interfaces;

namespace BPT_Service.Application.CategoryService.Query.GetByIDCategoryService
{
    public class GetByIDCategoryServiceQuery : IGetByIDCategoryServiceQuery
    {
        private readonly IRepository<Category, int> _categoryRepository;
        public GetByIDCategoryServiceQuery(IRepository<Category, int> categoryRepository)
        {
            _categoryRepository = categoryRepository;
        }
        public async Task<CategoryServiceViewModel> ExecuteAsync(int id)
        {
            var CategoryItem = await _categoryRepository.FindByIdAsync(id);
            CategoryServiceViewModel categoryViewModels = new CategoryServiceViewModel
            {
                Id = CategoryItem.Id,
                CategoryName = CategoryItem.CategoryName,
                NameVietnamese = CategoryItem.NameVietnamese,
                Description = CategoryItem.Description
            };
            return categoryViewModels;
        }
    }
}