using System.Collections.Generic;
using System.Threading.Tasks;
using BPT_Service.Application.ViewModels.System;
using BPT_Service.Model.Entities;

namespace BPT_Service.Application.Interfaces
{
    public interface IAuthenticateService
    {
        Task<AppUser> Authenticate(string username, string password);
        Task<IEnumerable<AppUserViewModel>> GetAll();
        Task<AppUserViewModel> GetById(string id);
    }
}