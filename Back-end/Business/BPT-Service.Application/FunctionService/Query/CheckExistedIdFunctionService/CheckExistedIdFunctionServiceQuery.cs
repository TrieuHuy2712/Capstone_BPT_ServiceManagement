using System.Threading.Tasks;
using BPT_Service.Model.Entities;
using BPT_Service.Model.Infrastructure.Interfaces;

namespace BPT_Service.Application.FunctionService.Query.CheckExistedIdFunctionService
{
    public class CheckExistedIdFunctionServiceQuery : ICheckExistedFunctionServiceQuery
    {
        private readonly IRepository<Function, string> _functionRepository;
        public CheckExistedIdFunctionServiceQuery(IRepository<Function, string> functionRepository)
        {
            _functionRepository = functionRepository;
        }
        public async Task<bool> ExecuteAsync(string id)
        {
            return await _functionRepository.FindByIdAsync(id) != null;
        }
    }
}