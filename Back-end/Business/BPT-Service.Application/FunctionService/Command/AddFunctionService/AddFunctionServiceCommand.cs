using System.Threading.Tasks;
using BPT_Service.Application.FunctionService.ViewModel;
using BPT_Service.Model.Entities;
using BPT_Service.Model.Infrastructure.Interfaces;

namespace BPT_Service.Application.FunctionService.Command.AddFunctionService
{
    public class AddFunctionServiceCommand : IAddFunctionServiceCommand
    {
        private readonly IRepository<Function, string> _functionRepository;

        public AddFunctionServiceCommand(
            IRepository<Function, string> functionRepository)
        {
            _functionRepository = functionRepository;
        }
        public async Task<CommandResult<FunctionViewModelinFunctionService>> ExecuteAsync(FunctionViewModelinFunctionService function)
        {
            try
            {
                var mappingFunction = MappingFunction(function);
                await _functionRepository.Add(mappingFunction);
                await _functionRepository.SaveAsync();
                return new CommandResult<FunctionViewModelinFunctionService>
                {
                    isValid = true,
                    myModel = new FunctionViewModelinFunctionService
                    {
                        IconCss = mappingFunction.IconCss,
                        Id = mappingFunction.Id,
                        Name = mappingFunction.Name,
                        ParentId = mappingFunction.ParentId,
                        SortOrder = mappingFunction.SortOrder,
                        Status = mappingFunction.Status,
                        URL = mappingFunction.URL
                    }
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
        
        public Function MappingFunction(FunctionViewModelinFunctionService function)
        {
            Function newfunction = new Function();
            newfunction.Id = function.Id;
            newfunction.IconCss = function.IconCss;
            newfunction.Name = function.Name;
            newfunction.ParentId = function.ParentId;
            newfunction.SortOrder = function.SortOrder;
            newfunction.Status = function.Status;
            newfunction.URL = function.URL;
            return newfunction;
        }
    }
}