using BPT_Service.Application.ProviderService.Command.ApproveProviderService;
using BPT_Service.Application.ProviderService.Command.ConfirmProviderService;
using BPT_Service.Application.ProviderService.Command.DeleteProviderService;
using BPT_Service.Application.ProviderService.Command.RegisterProviderService;
using BPT_Service.Application.ProviderService.Command.RejectProviderService;
using BPT_Service.Application.ProviderService.Command.UpdateProviderService;
using BPT_Service.Application.ProviderService.Query.CheckUserIsProvider;
using BPT_Service.Application.ProviderService.Query.GetAllPagingProviderService;
using BPT_Service.Application.ProviderService.Query.GetAllProviderofUserService;
using BPT_Service.Application.ProviderService.Query.GetByIdProviderService;
using BPT_Service.Application.ProviderService.ViewModel;
using BPT_Service.WebAPI.Models.ProviderViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

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
        private readonly ICheckUserIsProviderQuery _checkUserIsProviderQuery;
        private readonly IUpdateProviderServiceCommand _updateProviderServiceCommand;
        private readonly IConfirmProviderService _confirmProviderService;

        public ProviderController(IApproveProviderServiceCommand approveProviderServiceCommand,
        IDeleteProviderServiceCommand deleteProviderServiceCommand,
        IRegisterProviderServiceCommand registerProviderServiceCommand,
        IRejectProviderServiceCommand rejectProviderServiceCommand,
        IGetAllPagingProviderServiceQuery getAllPagingProviderServiceQuery,
        IGetAllProviderofUserServiceQuery getAllProviderofUserServiceQuery,
        IGetByIdProviderServiceQuery getByIdProviderServiceQuery,
        IUpdateProviderServiceCommand updateProviderServiceCommand,
        ICheckUserIsProviderQuery checkUserIsProviderQuery,
        IConfirmProviderService confirmProviderService)
        {
            _approveProviderServiceCommand = approveProviderServiceCommand;
            _deleteProviderServiceCommand = deleteProviderServiceCommand;
            _registerProviderServiceCommand = registerProviderServiceCommand;
            _rejectProviderServiceCommand = rejectProviderServiceCommand;
            _getAllPagingProviderServiceQuery = getAllPagingProviderServiceQuery;
            _getAllProviderofUserServiceQuery = getAllProviderofUserServiceQuery;
            _getByIdProviderServiceQuery = getByIdProviderServiceQuery;
            _checkUserIsProviderQuery = checkUserIsProviderQuery;
            _updateProviderServiceCommand = updateProviderServiceCommand;
            _confirmProviderService = confirmProviderService;
        }

        [HttpGet("GetAllPaging")]
        public async Task<IActionResult> GetAllPaging(string keyword, int page, int pageSize, int filter)
        {
            var model = await _getAllPagingProviderServiceQuery.ExecuteAsync(keyword, page, pageSize, filter);
            return new OkObjectResult(model);
        }

        [AllowAnonymous]
        [HttpGet("ConfirmProvider/{codeConfirm}")]
        public async Task<IActionResult> GetAllPaging(string codeConfirm)
        {
            var model = await _confirmProviderService.ExecuteAsync(codeConfirm);
            return new OkObjectResult(model);
        }

        [HttpGet("GetProviderById/{id}")]
        public async Task<IActionResult> GetProviderById(string id)
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
        public async Task<IActionResult> DeleteAProvider(string id)
        {
            var model = await _deleteProviderServiceCommand.ExecuteAsync(id);
            return new OkObjectResult(model);
        }

        [HttpPost("RegisterProvider")]
        public async Task<IActionResult> RegisterAProvider(ProviderServiceViewModel vm)
        {
            var model = await _registerProviderServiceCommand.ExecuteAsync(vm);
            return new OkObjectResult(model);
        }

        [HttpPost("ApproveProvider")]
        public async Task<IActionResult> ApproveAProvider(ProviderServiceViewModel vm)
        {
            var model = await _approveProviderServiceCommand.ExecuteAsync(vm.UserId, vm.Id);
            return new OkObjectResult(model);
        }

        [HttpPost("RejectProvider")]
        public async Task<IActionResult> RejectAProvider(ProviderServiceViewModel vm)
        {
            var model = await _rejectProviderServiceCommand.ExecuteAsync(vm.Id, vm.Reason);
            return new OkObjectResult(model);
        }

        [HttpGet("CheckUserIsProvider")]
        public async Task<IActionResult> CheckUserIsProvider(string userId)
        {
            var model = await _checkUserIsProviderQuery.ExecuteAsync(userId);
            return new OkObjectResult(model);
        }

        [HttpPost("UpdateProviderService")]
        public async Task<IActionResult> UpdateProviderServiceCommand([FromBody]ProviderServiceViewModel vm)
        {
            var model = await _updateProviderServiceCommand.ExecuteAsync(vm);
            return new OkObjectResult(model);
        }
    }
}