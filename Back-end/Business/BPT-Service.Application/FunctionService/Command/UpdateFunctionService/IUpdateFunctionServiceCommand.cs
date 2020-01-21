using System.Threading.Tasks;
using BPT_Service.Application.FunctionService.ViewModel;

namespace BPT_Service.Application.FunctionService.Command.UpdateFunctionService
{
    public interface IUpdateFunctionServiceCommand
    {
         Task<bool> ExecuteAsync(FunctionViewModelinFunctionService function);
    }
}