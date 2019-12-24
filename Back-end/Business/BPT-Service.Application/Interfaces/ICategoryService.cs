using BPT_Service.Application.ViewModels.System;
using BPT_Service.Common.Dtos;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace BPT_Service.Application.Interfaces
{
    public interface ICategoryService
    {
        #region Interface get
        Task<List<CategoryViewModel>> GetAllAsync();

        PagedResult<CategoryViewModel> GetAllPagingAsync(string keyword, int page, int pageSize);

        Task<CategoryViewModel> GetByID(int id);
        #endregion

        #region Add
        Task<bool> AddAsync(CategoryViewModel userVm);
        #endregion

        #region Update
        Task<bool> Update(CategoryViewModel userVm);
        #endregion

        #region Delete
        Task<bool> DeleteAsync(int id);
        #endregion

        void Save();


    }
}
