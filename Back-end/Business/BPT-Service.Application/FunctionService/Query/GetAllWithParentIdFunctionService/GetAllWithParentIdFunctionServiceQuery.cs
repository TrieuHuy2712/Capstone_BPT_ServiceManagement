using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BPT_Service.Application.FunctionService.ViewModel;
using BPT_Service.Model.Entities;
using BPT_Service.Model.Infrastructure.Interfaces;

namespace BPT_Service.Application.FunctionService.Query.GetAllWithParentIdFunctionService
{
    public class GetAllWithParentIdFunctionServiceQuery : IGetAllWithParentIdFunctionServiceQuery
    {
        private readonly IRepository<Function, string> _functionRepository;
        public GetAllWithParentIdFunctionServiceQuery(IRepository<Function, string> functionRepository)
        {
            _functionRepository = functionRepository;
        }
        public async Task<IEnumerable<FunctionViewModelinFunctionService>> ExecuteAsync(string parentId)
        {
            var allFunction = await _functionRepository.FindAllAsync(x => x.ParentId == parentId);
            return allFunction.Select(x => new FunctionViewModelinFunctionService
            {
                IconCss = x.IconCss,
                Id = x.Id,
                Name = x.Name,
                ParentId = x.ParentId,
                Status = x.Status,
                URL = x.URL
            });
        }

    }
}