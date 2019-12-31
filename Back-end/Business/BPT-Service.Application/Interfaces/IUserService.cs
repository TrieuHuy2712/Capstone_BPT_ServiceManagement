using System.Collections.Generic;
using System.Threading.Tasks;
using BPT_Service.Application.ViewModels.System;
using BPT_Service.Common.Dtos;

namespace BPT_Service.Application.Interfaces
{
    public interface IUserService
    {
        Task<bool> AddAsync(AppUserViewModel userVm);

        Task<bool> DeleteAsync(string id);

        Task<List<AppUserViewModel>> GetAllAsync();

        PagedResult<AppUserViewModel> GetAllPagingAsync(string keyword, int page, int pageSize);

        Task<AppUserViewModel> GetById(string id);

        Task<bool> CreateCustomerAsync(AppUserViewModel userVm, string password);

        Task<bool> UpdateAsync(AppUserViewModel userVm);

        Task<AppUserViewModel> AddExternalAsync(AppUserViewModel socialUserViewModel);
    }
}