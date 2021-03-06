using System;
using System.Threading.Tasks;
using BPT_Service.Application.PostService.ViewModel;
using BPT_Service.Model.Entities;

namespace BPT_Service.Application.PostService.Query.GetPostServiceById
{
    public interface IGetPostServiceByIdQuery
    {
         Task<PostServiceViewModel> ExecuteAsync(string idService);
    }
}