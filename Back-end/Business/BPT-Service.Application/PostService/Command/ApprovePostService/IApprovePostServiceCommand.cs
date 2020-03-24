using System.Threading.Tasks;
using BPT_Service.Application.PostService.ViewModel;
using BPT_Service.Model.Entities;

namespace BPT_Service.Application.PostService.Command.ApprovePostService
{
    public interface IApprovePostServiceCommand
    {
        Task<CommandResult<PostServiceViewModel>> ExecuteAsync(string idService);
    }
}