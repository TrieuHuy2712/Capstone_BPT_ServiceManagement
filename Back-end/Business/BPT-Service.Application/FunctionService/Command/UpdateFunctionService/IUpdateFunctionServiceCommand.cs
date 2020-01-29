using System.Threading.Tasks;
using BPT_Service.Application.FunctionService.ViewModel;
using BPT_Service.Model.Entities;

namespace BPT_Service.Application.FunctionService.Command.UpdateFunctionService
{
    public interface IUpdateFunctionServiceCommand
    {
         Task<CommandResult<FunctionViewModelinFunctionService>> ExecuteAsync(FunctionViewModelinFunctionService function);
    }
}