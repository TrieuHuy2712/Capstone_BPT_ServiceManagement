using System;
using System.Threading.Tasks;
using BPT_Service.Application.FunctionService.ViewModel;

namespace BPT_Service.Application.FunctionService.Command.AddFunctionService
{
    public interface IAddFunctionServiceCommand
    {
        Task<bool> ExecuteAsync(FunctionViewModelinFunctionService function);
    }
}