using BPT_Service.Application.PostService.ViewModel;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BPT_Service.Application.PostService.Query.GetAllPostUserServiceByUserId
{
    public interface IGetAllPostUserServiceByUserIdQuery
    {
        Task<List<ListServiceViewModel>> ExecuteAsync(string idUser, bool isProvider);
    }
}