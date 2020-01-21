using System.Collections.Generic;
using System.Threading.Tasks;

namespace BPT_Service.Application.FunctionService.Command.UpdateParentId
{
    public interface IUpdateParentIdServiceCommand
    {
        Task<bool> ExecuteAsync(string sourceId, string targetId, Dictionary<string, int> items);
    }
}