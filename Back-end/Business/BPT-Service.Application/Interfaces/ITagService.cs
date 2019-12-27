using BPT_Service.Application.ViewModels.System;
using BPT_Service.Common.Dtos;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace BPT_Service.Application.Interfaces
{
    public interface ITagService
    {
        #region Interface get
        Task<List<TagViewModel>> GetAllAsync();

        PagedResult<TagViewModel> GetAllPagingAsync(string keyword, int page, int pageSize);

        Task<TagViewModel> GetByID(int id);
        #endregion

        #region Add
        Task<bool> AddAsync(TagViewModel userVm);
        #endregion

        #region Update
        Task<bool> Update(TagViewModel userVm);
        #endregion

        #region Delete
        Task<bool> DeleteAsync(int id);
        #endregion

        void Save();


    }
}
