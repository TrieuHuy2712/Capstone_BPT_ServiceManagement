using System.Collections.Generic;
using System.Threading.Tasks;
using BPT_Service.Application.FunctionService.ViewModel;
using BPT_Service.Model.Entities;
using BPT_Service.Model.Infrastructure.Interfaces;

namespace BPT_Service.Application.FunctionService.Command.UpdateParentId
{

    public class UpdateParentIdServiceCommand : IUpdateParentIdServiceCommand
    {
        private readonly IRepository<Function, string> _functionRepository;
        public UpdateParentIdServiceCommand(IRepository<Function, string> functionRepository)
        {
            _functionRepository = functionRepository;
        }
        public async Task<CommandResult<FunctionViewModelinFunctionService>> ExecuteAsync(string sourceId, string targetId, Dictionary<string, int> items)
        {
            try
            {
                //Update parent id for source
                var category = await _functionRepository.FindByIdAsync(sourceId);
                if (category != null)
                {
                    category.ParentId = targetId;
                    _functionRepository.Update(category);

                    //Get all sibling
                    var sibling = await _functionRepository.FindAllAsync(x => items.ContainsKey(x.Id));
                    foreach (var child in sibling)
                    {
                        child.SortOrder = items[child.Id];
                        _functionRepository.Update(child);
                    }
                    await _functionRepository.SaveAsync();
                    return new CommandResult<FunctionViewModelinFunctionService>
                    {
                        isValid = true,
                        myModel = new FunctionViewModelinFunctionService
                        {
                            IconCss = category.IconCss,
                            Id = category.Id,
                            Name = category.Name,
                            ParentId = category.ParentId,
                            SortOrder = category.SortOrder,
                            Status = category.Status,
                            URL = category.URL
                        }
                    };
                }
                return new CommandResult<FunctionViewModelinFunctionService>
                {
                    isValid = false,
                    errorMessage = "Cannot find Source ID"
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