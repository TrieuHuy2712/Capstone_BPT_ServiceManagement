using BPT_Service.Application.Interfaces;
using BPT_Service.Application.ViewModels.System;
using BPT_Service.Common.Dtos;
using BPT_Service.Model.Entities;
using BPT_Service.Model.Infrastructure.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BPT_Service.Application.Implementation
{
    public class CategoryService : ICategoryService
    {
        private IRepository<Category, int> _categoryRepository;
        private IUnitOfWork _unitOfWork;
        public CategoryService(IRepository<Category, int> categoryRepository, IUnitOfWork unitOfWork)
        {
            _categoryRepository = categoryRepository;
            _unitOfWork = unitOfWork;
        }

        #region Add
        public async Task<bool> AddAsync(CategoryViewModel userVm)
        {
            Category category = new Category();
            category.Id = userVm.Id;
            category.CategoryName = userVm.CategoryName;
            category.NameVietnamese = userVm.NameVietnamese;
            category.Description = userVm.Description;
            _categoryRepository.Add(category);
            return true;
        }

        #endregion

        #region Delete
        public async Task<bool> DeleteAsync(int id)
        {
            var CategoryDel = _categoryRepository.FindById(id);
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


        #endregion

        #region Get
        public async Task<List<CategoryViewModel>> GetAllAsync()
        {
            var listCategory = _categoryRepository.FindAll();
            var categoryViewModels = await listCategory.Select(x => new CategoryViewModel
            {
                Id = x.Id,
                CategoryName = x.CategoryName,
                Description = x.Description,
                NameVietnamese = x.NameVietnamese
            }).ToListAsync();
            return categoryViewModels;
        }

        public PagedResult<CategoryViewModel> GetAllPagingAsync(string keyword, int page, int pageSize)
        {
            var query = _categoryRepository.FindAll();
            if (!string.IsNullOrEmpty(keyword))
                query = query.Where(x => x.CategoryName.Contains(keyword)
                || x.Description.Contains(keyword));

            int totalRow = query.Count();
            query = query.Skip((page - 1) * pageSize)
               .Take(pageSize);

            var data = query.Select(x => new CategoryViewModel
            {
                Id = x.Id,
                CategoryName = x.CategoryName,
                Description = x.Description,
                NameVietnamese = x.NameVietnamese
            }).ToList();

            var paginationSet = new PagedResult<CategoryViewModel>()
            {
                Results = data,
                CurrentPage = page,
                RowCount = totalRow,
                PageSize = pageSize
            };

            return paginationSet;
        }

        public async Task<CategoryViewModel> GetByID(int id)
        {
            var CategoryItem = _categoryRepository.FindById(id);
            CategoryViewModel categoryViewModels = new CategoryViewModel();
            categoryViewModels.Id = CategoryItem.Id;
            categoryViewModels.CategoryName = CategoryItem.CategoryName;
            categoryViewModels.NameVietnamese = CategoryItem.NameVietnamese;
            categoryViewModels.Description = CategoryItem.Description;
            return categoryViewModels;
        }
        #endregion

        #region Update
        public async Task<bool> Update(CategoryViewModel userVm)
        {
            try
            {
                var CategoryUpdate = _categoryRepository.FindById(userVm.Id);
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
            catch (System.Exception ex)
            {

                return false;
            }

        }

        #endregion

        public void Save()
        {
            _unitOfWork.Commit();
        }

    }
}
