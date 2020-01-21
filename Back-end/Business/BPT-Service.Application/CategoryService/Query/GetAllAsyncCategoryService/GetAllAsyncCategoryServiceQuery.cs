using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BPT_Service.Application.CategoryService.ViewModel;
using BPT_Service.Model.Entities;
using BPT_Service.Model.Infrastructure.Interfaces;

namespace BPT_Service.Application.CategoryService.Query.GetAllAsyncCategoryService
{
    public class GetAllAsyncCategoryServiceQuery : IGetAllAsyncCategoryServiceQuery
    {
        private readonly IRepository<Category, int> _categoryRepository;
        public GetAllAsyncCategoryServiceQuery(IRepository<Category, int> categoryRepository, IUnitOfWork unitOfWork)
        {
            _categoryRepository = categoryRepository;
        }
        public async Task<List<CategoryServiceViewModel>> ExecuteAsync()
        {
            var listCategory = await _categoryRepository.FindAllAsync();
            var categoryViewModels = listCategory.Select(x => new CategoryServiceViewModel
            {
                Id = x.Id,
                CategoryName = x.CategoryName,
                Description = x.Description,
                NameVietnamese = x.NameVietnamese
            }).ToList();
            return categoryViewModels;
        }
    }
}