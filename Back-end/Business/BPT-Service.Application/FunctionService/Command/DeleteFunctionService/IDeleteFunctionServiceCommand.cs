using System;
using System.Threading.Tasks;

namespace BPT_Service.Application.FunctionService.Command.DeleteFunctionService
{
    public interface IDeleteFunctionServiceCommand
    {
        Task<bool> ExecuteAsync(string id);
    }
}