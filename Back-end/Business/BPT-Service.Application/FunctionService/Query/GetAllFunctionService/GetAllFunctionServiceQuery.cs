using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BPT_Service.Application.FunctionService.ViewModel;
using BPT_Service.Model.Entities;
using BPT_Service.Model.Enums;
using BPT_Service.Model.Infrastructure.Interfaces;

namespace BPT_Service.Application.FunctionService.Query.GetAllFunctionService
{
    public class GetAllFunctionServiceQuery : IGetAllFunctionServiceQuery
    {
        private readonly IRepository<Function, string> _functionRepository;
        public GetAllFunctionServiceQuery(IRepository<Function, string> functionRepository)
        {
            _functionRepository = functionRepository;
        }
        public async Task<List<FunctionViewModelinFunctionService>> ExecuteAsync(string filter)
        {
            var query = await _functionRepository.FindAllAsync(x => x.Status == Status.Active);
            if (!string.IsNullOrEmpty(filter))
                query = query.Where(x => x.Name.Contains(filter));
            return (query.OrderBy(x => x.ParentId).Select(x => new FunctionViewModelinFunctionService
            {
                IconCss = x.IconCss,
                Id = x.Id,
                Name = x.Name,
                ParentId = x.ParentId,
                SortOrder = x.SortOrder,
                Status = x.Status,
                NameVietNamese = x.NameVietNamese,
                URL = x.URL
            })).ToList();
        }
    }
}