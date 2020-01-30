using System;
using System.Threading.Tasks;
using BPT_Service.Application.PostService.ViewModel;
using BPT_Service.Model.Entities;

namespace BPT_Service.Application.PostService.Command.PostServiceFromUser.DeleteServiceFromUser
{
    public interface IDeleteServiceFromUserCommand
    {
        Task<CommandResult<PostServiceViewModel>> ExecuteAsync(Guid idService);
    }
}