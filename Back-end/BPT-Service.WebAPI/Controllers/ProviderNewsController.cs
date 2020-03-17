using BPT_Service.Application.NewsProviderService.Command.ApproveNewsProvider;
using BPT_Service.Application.NewsProviderService.Command.DeleteNewsProviderService;
using BPT_Service.Application.NewsProviderService.Command.RegisterNewsProviderService;
using BPT_Service.Application.NewsProviderService.Command.RejectNewsProvider;
using BPT_Service.Application.NewsProviderService.Command.UpdateNewsProviderService;
using BPT_Service.Application.NewsProviderService.Query.GetAllPagingProviderNewsOfProvider;
using BPT_Service.Application.NewsProviderService.Query.GetAllPagingProviderNewsService;
using BPT_Service.Application.NewsProviderService.Query.GetByIdProviderNewsService;
using BPT_Service.Application.NewsProviderService.ViewModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace BPT_Service.WebAPI.Controllers
{
    [Authorize]
    [ApiController]
    [Route("ProviderNews")]
    public class ProviderNewsController : ControllerBase
    {
        private readonly IApproveNewsProviderServiceCommand _approveProviderServiceCommand;
        private readonly IDeleteNewsProviderServiceCommand _deleteNewsProviderServiceCommand;
        private readonly IGetAllPagingProviderNewsOfProviderQuery _getAllPagingProviderNewsOfProviderQuery;
        private readonly IGetAllPagingProviderNewsServiceQuery _getAllPagingProviderNewsServiceQuery;
        private readonly IGetByIdProviderNewsServiceQuery _getByIdProviderNewsServiceQuery;
        private readonly IRegisterNewsProviderServiceCommand _registerNewsProviderServiceCommand;
        private readonly IRejectNewsProviderServiceCommand _rejectNewsProviderServiceCommand;
        private readonly IUpdateNewsProviderServiceCommand _updateNewsProviderServiceCommand;

        public ProviderNewsController(IApproveNewsProviderServiceCommand approveProviderServiceCommand,
        IDeleteNewsProviderServiceCommand deleteNewsProviderServiceCommand,
        IGetAllPagingProviderNewsOfProviderQuery getAllPagingProviderNewsOfProviderQuery,
        IGetAllPagingProviderNewsServiceQuery getAllPagingProviderNewsServiceQuery,
        IGetByIdProviderNewsServiceQuery getByIdProviderNewsServiceQuery,
        IRegisterNewsProviderServiceCommand registerNewsProviderServiceCommand,
        IRejectNewsProviderServiceCommand rejectNewsProviderServiceCommand,
        IUpdateNewsProviderServiceCommand updateNewsProviderServiceCommand)
        {
            _approveProviderServiceCommand = approveProviderServiceCommand;
            _deleteNewsProviderServiceCommand = deleteNewsProviderServiceCommand;
            _getAllPagingProviderNewsOfProviderQuery = getAllPagingProviderNewsOfProviderQuery;
            _getAllPagingProviderNewsServiceQuery = getAllPagingProviderNewsServiceQuery;
            _getByIdProviderNewsServiceQuery = getByIdProviderNewsServiceQuery;
            _registerNewsProviderServiceCommand = registerNewsProviderServiceCommand;
            _rejectNewsProviderServiceCommand = rejectNewsProviderServiceCommand;
            _updateNewsProviderServiceCommand = updateNewsProviderServiceCommand;
        }

        [HttpPost("ApproveNewsProvider")]
        public async Task<IActionResult> ApproveNewsProvider(int id)
        {
            var model = await _approveProviderServiceCommand.ExecuteAsync(id);

            return new OkObjectResult(model);
        }

        [HttpDelete("DeleteNewsProvider/{id}")]
        public async Task<IActionResult> DeleteNewsProvider(int id)
        {
            var model = await _deleteNewsProviderServiceCommand.ExecuteAsync(id);

            return new OkObjectResult(model);
        }

        [HttpGet("GetAllPagingProviderNewsOfProvider")]
        public async Task<IActionResult> GetAllPagingProviderNewsOfProvider(string keyword, int page, int pageSize)
        {
            var model = await _getAllPagingProviderNewsOfProviderQuery.ExecuteAsync(keyword, page, pageSize);

            return new OkObjectResult(model);
        }

        [HttpGet("GetAllPagingProviderNews")]
        public async Task<IActionResult> GetAllPagingProviderNews(string keyword, int page, int pageSize)
        {
            var model = await _getAllPagingProviderNewsServiceQuery.ExecuteAsync(keyword, page, pageSize);

            return new OkObjectResult(model);
        }

        [HttpGet("GetByIdProviderNewsServiceQuery")]
        public async Task<IActionResult> GetByIdProviderNewsServiceQuery(int id)
        {
            var model = await _getByIdProviderNewsServiceQuery.ExecuteAsync(id);

            return new OkObjectResult(model);
        }

        [HttpPost("RegisterNewsProvider")]
        public async Task<IActionResult> RegisterNewsProvider(NewsProviderViewModel vm)
        {
            var model = await _registerNewsProviderServiceCommand.ExecuteAsync(vm);

            return new OkObjectResult(model);
        }

        [HttpPost("RejectNewsProvider")]
        public async Task<IActionResult> RejectNewsProvider(NewsProviderViewModel vm)
        {
            var model = await _rejectNewsProviderServiceCommand.ExecuteAsync(vm);
            return new OkObjectResult(model);
        }

        [HttpPost("UpdateNewsProvider")]
        public async Task<IActionResult> UpdateNewsProvider(NewsProviderViewModel vm)
        {
            var model = await _updateNewsProviderServiceCommand.ExecuteAsync(vm);
            return new OkObjectResult(model);
        }
    }
}