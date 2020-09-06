using BPT_Service.Application.ElasticSearchService.Command.AddAllService;
using BPT_Service.Application.ElasticSearchService.Command.DeleteAllService;
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
        private readonly IDeleteAllService _deleteAllService;
        private readonly IAddAllService _addAllService;

        public ElasticSearchController(
            IFakeImportService fakeImportService,
            ISearchService searchService,
            IDeleteAllService deleteAllService,
            IAddAllService addAllService)
        {
            _fakeImportService = fakeImportService;
            _searchService = searchService;
            _deleteAllService = deleteAllService;
            _addAllService = addAllService;
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

        [HttpDelete("DeleteAllService")]
        public async Task<IActionResult> DeleteAllService()
        {
            var model = await _deleteAllService.ExecuteAsync();
            return new ObjectResult(model);
        }

        [HttpPost("AddAllService")]
        public async Task<IActionResult> AddAllService()
        {
            var model = await _addAllService.ExecuteAsync();
            return new ObjectResult(model);
        }
    }
}