using System;
using System.Threading.Tasks;
using BPT_Service.Application.ProviderService.Command.ApproveProviderService;
using BPT_Service.Application.ProviderService.Command.DeleteProviderService;
using BPT_Service.Application.ProviderService.Command.RegisterProviderService;
using BPT_Service.Application.ProviderService.Command.RejectProviderService;
using BPT_Service.Application.ProviderService.Query.GetAllPagingProviderService;
using BPT_Service.Application.ProviderService.Query.GetAllProviderofUserService;
using BPT_Service.Application.ProviderService.Query.GetByIdProviderService;
using BPT_Service.Application.ProviderService.ViewModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BPT_Service.WebAPI.Controllers
{
    [Authorize]
    [ApiController]
    [Route("[controller]")]
    public class ProviderController : ControllerBase
    {
        private readonly IApproveProviderServiceCommand _approveProviderServiceCommand;
        private readonly IDeleteProviderServiceCommand _deleteProviderServiceCommand;
        private readonly IRegisterProviderServiceCommand _registerProviderServiceCommand;
        private readonly IRejectProviderServiceCommand _rejectProviderServiceCommand;
        private readonly IGetAllPagingProviderServiceQuery _getAllPagingProviderServiceQuery;
        private readonly IGetAllProviderofUserServiceQuery _getAllProviderofUserServiceQuery;
        private readonly IGetByIdProviderServiceQuery _getByIdProviderServiceQuery;
        public ProviderController(IApproveProviderServiceCommand approveProviderServiceCommand,
        IDeleteProviderServiceCommand deleteProviderServiceCommand,
        IRegisterProviderServiceCommand registerProviderServiceCommand,
        IRejectProviderServiceCommand rejectProviderServiceCommand,
        IGetAllPagingProviderServiceQuery getAllPagingProviderServiceQuery,
        IGetAllProviderofUserServiceQuery getAllProviderofUserServiceQuery,
        IGetByIdProviderServiceQuery getByIdProviderServiceQuery)
        {
            _approveProviderServiceCommand = approveProviderServiceCommand;
            _deleteProviderServiceCommand = deleteProviderServiceCommand;
            _registerProviderServiceCommand = registerProviderServiceCommand;
            _rejectProviderServiceCommand = rejectProviderServiceCommand;
            _getAllPagingProviderServiceQuery = getAllPagingProviderServiceQuery;
            _getAllProviderofUserServiceQuery = getAllProviderofUserServiceQuery;
            _getByIdProviderServiceQuery = getByIdProviderServiceQuery;
        }

        [HttpGet("GetAllPaging")]
        public async Task<IActionResult> GetAllPaging(string keyword, int page, int pageSize)
        {
            var model = await _getAllPagingProviderServiceQuery.ExecuteAsync(keyword, page, pageSize);
            return new OkObjectResult(model);
        }

        [HttpGet("GetProviderById")]
        public async Task<IActionResult> GetProviderById(Guid id)
        {
            var model = await _getByIdProviderServiceQuery.ExecuteAsync(id);
            return new OkObjectResult(model);
        }

        [HttpGet("GetProviderUser")]
        public async Task<IActionResult> GetProviderUser()
        {
            var model = await _getAllProviderofUserServiceQuery.ExecuteAsync();
            return new OkObjectResult(model);
        }

        [HttpDelete("DeleteProvider")]
        public async Task<IActionResult> DeleteAProvider(Guid id)
        {
            var model = await _deleteProviderServiceCommand.ExecuteAsync(id);
            return new OkObjectResult(model);
        }

        [HttpPost("RegisterProvider")]
        public async Task<IActionResult> RegisterAProvider(ProviderServiceViewModel vm){
            var model= await _registerProviderServiceCommand.ExecuteAsync(vm);
            return new OkObjectResult(model);
        }

        [HttpPost("ApproveProvider")]
        public async Task<IActionResult> ApproveAProvider(ProviderServiceViewModel vm){
            var model= await _approveProviderServiceCommand.ExecuteAsync(vm);
            return new OkObjectResult(model);
        }

        [HttpPost("RejectProvider")]
        public async Task<IActionResult> RejectAProvider(ProviderServiceViewModel vm){
            var model= await _rejectProviderServiceCommand.ExecuteAsync(vm);
            return new OkObjectResult(model);
        }
    }
}