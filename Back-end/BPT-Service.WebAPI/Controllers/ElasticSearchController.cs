using BPT_Service.Application.ElasticSearchService.Command.FakeImport;
using BPT_Service.Application.ElasticSearchService.Query;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace BPT_Service.WebAPI.Controllers
{
    [Route("ServiceSearch")]
    public class ElasticSearchController : ControllerBase
    {
        private readonly IFakeImportService _fakeImportService;
        private readonly ISearchService _searchService;

        public ElasticSearchController(
            IFakeImportService fakeImportService,
            ISearchService searchService)
        {
            _fakeImportService = fakeImportService;
            _searchService = searchService;
        }

        [HttpGet("SearchService")]
        public async Task<IActionResult> SearchService(string query, int page = 1, int pageSize = 5)
        {
            var model = await _searchService.ExecuteAsync(query, page, pageSize);
            return new OkObjectResult(model);
        }
        
        [HttpGet("FakeImportService")]
        public async Task<IActionResult> FakeImportService(int count)
        {
            var model = await _fakeImportService.ExecuteAsync(count);
            return new OkObjectResult(model);
        }
    }
}