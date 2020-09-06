using BPT_Service.Application.PostService.ViewModel;
using System.Threading.Tasks;

namespace BPT_Service.Application.PostService.Query.GetPostUserServiceByUserId
{
    public interface IGetPostUserServiceByUserIdQuery
    {
        Task<ListServiceViewModel> ExecuteAsync(string idUser);
    }
}