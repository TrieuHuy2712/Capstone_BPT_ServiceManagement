using System.Threading.Tasks;
using BPT_Service.Model.Entities;
using BPT_Service.Model.Infrastructure.Interfaces;

namespace BPT_Service.Application.FunctionService.Query.ReOrderFunctionService
{
    public class ReOrderFunctionServiceQuery : IReOrderFunctionServiceQuery
    {
        private readonly IRepository<Function, string> _functionRepository;
        public ReOrderFunctionServiceQuery(IRepository<Function, string> functionRepository)
        {
            _functionRepository = functionRepository;
        }
        public async Task<bool> ExecuteAsync(string sourceId, string targetId)
        {
            var source = await _functionRepository.FindByIdAsync(sourceId);
            var target = await _functionRepository.FindByIdAsync(targetId);

            _functionRepository.Update(source);
            _functionRepository.Update(target);
            return true;
        }
    }
}