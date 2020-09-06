using System.Threading.Tasks;
using BPT_Service.Application.FunctionService.ViewModel;
using BPT_Service.Model.Entities;
using BPT_Service.Model.Infrastructure.Interfaces;

namespace BPT_Service.Application.FunctionService.Query.GetByIdFunctionService
{
    public class GetByIdFunctionServiceQuery : IGetByIdFunctionServiceQuery
    {
        private readonly IRepository<Function, string> _functionRepository;
        public GetByIdFunctionServiceQuery(IRepository<Function, string> functionRepository)
        {
            _functionRepository = functionRepository;
        }
        public async Task<FunctionViewModelinFunctionService> ExecuteAsync(string id)
        {
            var function = await _functionRepository.FindSingleAsync(x => x.Id == id);

            FunctionViewModelinFunctionService functionViewModel = new FunctionViewModelinFunctionService
            {
                IconCss = function.IconCss,
                Id = function.Id,
                Name = function.Name,
                ParentId = function.ParentId,
                Status = function.Status,
                URL = function.URL
            };
            return functionViewModel;
        }
    }
}