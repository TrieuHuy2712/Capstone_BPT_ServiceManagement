using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BPT_Service.Application.FunctionService.ViewModel;
using BPT_Service.Model.Entities;
using BPT_Service.Model.Infrastructure.Interfaces;

namespace BPT_Service.Application.FunctionService.Command.DeleteFunctionService
{
    public class DeleteFunctionServiceCommand : IDeleteFunctionServiceCommand
    {
        private readonly IRepository<Function, string> _functionRepository;
        public DeleteFunctionServiceCommand(IRepository<Function, string> functionRepository)
        {
            _functionRepository = functionRepository;
        }
        public async Task<CommandResult<FunctionViewModelinFunctionService>> ExecuteAsync(string id)
        {
            try
            {
                var getChildItem = await _functionRepository.FindAllAsync(x => x.ParentId == id && x.ParentId != null);
                if (getChildItem.Count() > 0)
                {
                    foreach (var item in getChildItem)
                    {
                        item.ParentId = null;
                        _functionRepository.Update(item);
                    }
                }
                _functionRepository.Remove(id);
                await _functionRepository.SaveAsync();
                return new CommandResult<FunctionViewModelinFunctionService>
                {
                    isValid = true,
                    myModel = null
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