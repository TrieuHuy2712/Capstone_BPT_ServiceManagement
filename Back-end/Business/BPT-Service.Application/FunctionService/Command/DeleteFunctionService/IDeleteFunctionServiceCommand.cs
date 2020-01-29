using System;
using System.Threading.Tasks;
using BPT_Service.Application.FunctionService.ViewModel;
using BPT_Service.Model.Entities;

namespace BPT_Service.Application.FunctionService.Command.DeleteFunctionService
{
    public interface IDeleteFunctionServiceCommand
    {
        Task<CommandResult<FunctionViewModelinFunctionService>> ExecuteAsync(string id);
    }
}