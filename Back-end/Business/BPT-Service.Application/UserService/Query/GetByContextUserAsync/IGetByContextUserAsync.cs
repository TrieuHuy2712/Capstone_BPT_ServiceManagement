using BPT_Service.Application.UserService.ViewModel;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace BPT_Service.Application.UserService.Query.GetByContextUserAsync
{
    public interface IGetByContextUserAsync
    {
        Task<AppUserViewModelinUserService> ExcecuteAsync();
    }
}
