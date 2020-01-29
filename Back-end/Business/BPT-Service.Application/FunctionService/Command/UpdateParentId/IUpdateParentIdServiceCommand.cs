using System.Collections.Generic;
using System.Threading.Tasks;
using BPT_Service.Application.FunctionService.ViewModel;
using BPT_Service.Model.Entities;

namespace BPT_Service.Application.FunctionService.Command.UpdateParentId
{
    public interface IUpdateParentIdServiceCommand
    {
        Task<CommandResult<FunctionViewModelinFunctionService>> ExecuteAsync(string sourceId, string targetId, Dictionary<string, int> items);
    }
}