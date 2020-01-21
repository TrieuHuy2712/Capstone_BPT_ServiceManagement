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
        public async Task<bool> ExecuteAsync(CategoryServiceViewModel userVm)
        {
            try
            {
                var CategoryUpdate = await _categoryRepository.FindByIdAsync(userVm.Id);
                if (CategoryUpdate != null)
                {
                    CategoryUpdate.CategoryName = userVm.CategoryName;
                    CategoryUpdate.NameVietnamese = userVm.NameVietnamese;
                    CategoryUpdate.Description = userVm.Description;
                    _categoryRepository.Update(CategoryUpdate);
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}