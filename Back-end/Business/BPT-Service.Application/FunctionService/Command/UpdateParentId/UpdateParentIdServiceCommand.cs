using System.Collections.Generic;
using System.Threading.Tasks;
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
        public async Task<bool> ExecuteAsync(string sourceId, string targetId, Dictionary<string, int> items)
        {
            //Update parent id for source
            var category = await _functionRepository.FindByIdAsync(sourceId);
            category.ParentId = targetId;
            _functionRepository.Update(category);

            //Get all sibling
            var sibling = await _functionRepository.FindAllAsync(x => items.ContainsKey(x.Id));
            foreach (var child in sibling)
            {
                child.SortOrder = items[child.Id];
                _functionRepository.Update(child);
            }
            return true;
        }
    }
}