using System;
using System.Threading.Tasks;
using BPT_Service.Application.FunctionService.ViewModel;
using BPT_Service.Model.Entities;

namespace BPT_Service.Application.FunctionService.Command.AddFunctionService
{
    public interface IAddFunctionServiceCommand
    {
        Task<CommandResult<FunctionViewModelinFunctionService>> ExecuteAsync(FunctionViewModelinFunctionService function);
    }
}