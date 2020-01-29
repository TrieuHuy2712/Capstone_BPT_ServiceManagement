using System;
using System.Threading.Tasks;
using BPT_Service.Application.CategoryService.ViewModel;
using BPT_Service.Model.Entities;
using BPT_Service.Model.Infrastructure.Interfaces;

namespace BPT_Service.Application.CategoryService.Command.UpdateCategoryService
{
    public class UpdateCategoryServiceCommand : IUpdateCategoryServiceCommand
    {
        private readonly IRepository<Category, int> _categoryRepository;
        public UpdateCategoryServiceCommand(IRepository<Category, int> categoryRepository, IUnitOfWork unitOfWork)
        {
            _categoryRepository = categoryRepository;
        }
        public async Task<CommandResult<CategoryServiceViewModel>> ExecuteAsync(CategoryServiceViewModel userVm)
        {
            try
            {
                var CategoryUpdate = await _categoryRepository.FindByIdAsync(userVm.Id);
                if (CategoryUpdate != null)
                {
                    CategoryUpdate.CategoryName = userVm.CategoryName;
                    CategoryUpdate.Description = userVm.Description;
                    _categoryRepository.Update(CategoryUpdate);
                    await _categoryRepository.SaveAsync();
                    return new CommandResult<CategoryServiceViewModel>
                    {
                        isValid = true,
                        myModel = userVm,
                        errorMessage = "Can't not find your Id"
                    };
                }
                else
                {
                    return new CommandResult<CategoryServiceViewModel>
                    {
                        isValid = false,
                        myModel = userVm,
                    };
                }
            }
            catch (Exception ex)
            {
                return new CommandResult<CategoryServiceViewModel>
                {
                    isValid = false,
                    myModel = userVm,
                    errorMessage = ex.InnerException.ToString()
                };
            }
        }
    }
}