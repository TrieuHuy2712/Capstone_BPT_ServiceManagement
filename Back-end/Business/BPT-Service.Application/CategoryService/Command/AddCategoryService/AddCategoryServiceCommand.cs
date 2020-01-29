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
        public async Task<CommandResult<CategoryServiceViewModel>> ExecuteAsync(CategoryServiceViewModel userVm)
        {
            try
            {
                var mappingCate = mappingCategory(userVm);
                await _categoryRepository.Add(mappingCate);
                await _categoryRepository.SaveAsync();

                return new CommandResult<CategoryServiceViewModel>
                {
                    isValid = true,
                    myModel = new CategoryServiceViewModel
                    {
                        Id = mappingCate.Id,
                        CategoryName = mappingCate.CategoryName,
                        Description = mappingCate.Description,
                    },
                };
            }
            catch (System.Exception ex)
            {

                return new CommandResult<CategoryServiceViewModel>
                {
                    isValid = false,
                    myModel = userVm,
                    errorMessage = ex.InnerException.ToString()
                };
            }
        }
        
        private Category mappingCategory(CategoryServiceViewModel userVm)
        {
            Category category = new Category();
            category.Id = userVm.Id;
            category.CategoryName = userVm.CategoryName;
            category.Description = userVm.Description;
            return category;
        }
    }
}