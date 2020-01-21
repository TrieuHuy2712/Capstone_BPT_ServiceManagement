using System;
using System.Threading.Tasks;

namespace BPT_Service.Application.TagService.Command.DeleteServiceAsync
{
    public interface IDeleteTagServiceAsyncCommand
    {
        Task<bool> ExecuteAsync(Guid id);
    }
}