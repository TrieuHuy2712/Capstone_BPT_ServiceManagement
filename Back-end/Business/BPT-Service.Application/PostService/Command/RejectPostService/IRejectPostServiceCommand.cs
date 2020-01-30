using System.Threading.Tasks;
using BPT_Service.Application.PostService.ViewModel;
using BPT_Service.Model.Entities;

namespace BPT_Service.Application.PostService.Command.RejectPostService
{
    public interface IRejectPostServiceCommand
    {
         Task<CommandResult<PostServiceViewModel>> ExecuteAsync(PostServiceViewModel vm);
    }
}