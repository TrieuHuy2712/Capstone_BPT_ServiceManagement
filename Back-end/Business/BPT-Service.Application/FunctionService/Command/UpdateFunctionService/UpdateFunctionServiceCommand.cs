using System.Threading.Tasks;
using BPT_Service.Application.FunctionService.ViewModel;
using BPT_Service.Model.Entities;
using BPT_Service.Model.Infrastructure.Interfaces;

namespace BPT_Service.Application.FunctionService.Command.UpdateFunctionService
{
    public class UpdateFunctionServiceCommand : IUpdateFunctionServiceCommand
    {
        private readonly IRepository<Function, string> _functionRepository;
        public UpdateFunctionServiceCommand(IRepository<Function, string> functionRepository)
        {
            _functionRepository = functionRepository;
        }
        public async Task<CommandResult<FunctionViewModelinFunctionService>> ExecuteAsync(FunctionViewModelinFunctionService function)
        {
            try
            {
                var functionDb = await _functionRepository.FindByIdAsync(function.Id);

                if (functionDb != null)
                {
                    functionDb.IconCss = function.IconCss;
                    functionDb.Id = function.Id;
                    functionDb.Name = function.Name;
                    functionDb.ParentId = function.ParentId;
                    functionDb.SortOrder = function.SortOrder;
                    functionDb.Status = function.Status;
                    functionDb.URL = function.URL;
                    _functionRepository.Update(functionDb);
                    await _functionRepository.SaveAsync();
                    return new CommandResult<FunctionViewModelinFunctionService>
                    {
                        isValid = true,
                        myModel = function,
                    };
                }
                return new CommandResult<FunctionViewModelinFunctionService>
                {
                    isValid = false,
                    errorMessage = "Cannot not find your ID"
                };
            }
            catch (System.Exception ex)
            {
                return new CommandResult<FunctionViewModelinFunctionService>
                {
                    isValid = false,
                    errorMessage = ex.InnerException.ToString()
                };
            }
        }
    }
}