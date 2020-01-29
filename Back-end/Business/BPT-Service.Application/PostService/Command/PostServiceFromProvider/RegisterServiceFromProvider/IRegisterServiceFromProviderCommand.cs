using System.Threading.Tasks;
using BPT_Service.Application.PostService.ViewModel;
using BPT_Service.Model.Entities;

namespace BPT_Service.Application.PostService.Command.PostServiceFromProvider.RegisterServiceFromProvider
{
    public interface IRegisterServiceFromProviderCommand
    {
         Task<CommandResult<PostServiceViewModel>> ExecuteAsync(PostServiceViewModel vm);
    }
}