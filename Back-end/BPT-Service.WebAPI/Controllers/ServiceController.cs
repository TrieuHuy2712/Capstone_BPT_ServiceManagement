using System;
using System.Threading.Tasks;
using BPT_Service.Application.PostService.Command.ApprovePostService;
using BPT_Service.Application.PostService.Command.PostServiceFromProvider.DeleteServiceFromProvider;
using BPT_Service.Application.PostService.Command.PostServiceFromProvider.RegisterServiceFromProvider;
using BPT_Service.Application.PostService.Command.PostServiceFromUser.DeleteServiceFromUser;
using BPT_Service.Application.PostService.Command.PostServiceFromUser.RegisterServiceFromUser;
using BPT_Service.Application.PostService.Command.RejectPostService;
using BPT_Service.Application.PostService.Command.UpdatePostService;
using BPT_Service.Application.PostService.Query.GetAllPagingPostService;
using BPT_Service.Application.PostService.Query.GetPostServiceById;
using BPT_Service.Application.PostService.ViewModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BPT_Service.WebAPI.Controllers
{
    [Authorize]
    [ApiController]
    [Route("[controller]")]
    public class ServiceController : ControllerBase
    {
        private readonly IApprovePostServiceCommand _approvePostServiceCommand;
        private readonly IDeleteServiceFromProviderCommand _deleteServiceFromProviderCommand;
        private readonly IDeleteServiceFromUserCommand _deleteServiceFromUserCommand;
        private readonly IGetAllPagingPostServiceQuery _getAllPagingPostServiceQuery;
        private readonly IGetPostServiceByIdQuery _getPostServiceByIdQuery;
        private readonly IRegisterServiceFromProviderCommand _registerServiceFromProviderCommand;
        private readonly IRegisterServiceFromUserCommand _registerServiceFromUserCommand;
        private readonly IRejectPostServiceCommand _rejectPostServiceCommand;
        private readonly IUpdatePostServiceCommand _updatePostServiceCommand;
        public ServiceController(
            IApprovePostServiceCommand approvePostServiceCommand,
            IDeleteServiceFromProviderCommand deleteServiceFromProviderCommand,
            IDeleteServiceFromUserCommand deleteServiceFromUserCommand,
            IGetAllPagingPostServiceQuery getAllPagingPostServiceQuery,
            IGetPostServiceByIdQuery getPostServiceByIdQuery,
            IRegisterServiceFromProviderCommand registerServiceFromProviderCommand,
            IRegisterServiceFromUserCommand registerServiceFromUserCommand,
            IRejectPostServiceCommand rejectPostServiceCommand,
            IUpdatePostServiceCommand updatePostServiceCommand
        )
        {
            _approvePostServiceCommand = approvePostServiceCommand;
            _deleteServiceFromProviderCommand = deleteServiceFromProviderCommand;
            _deleteServiceFromUserCommand = deleteServiceFromUserCommand;
            _getAllPagingPostServiceQuery = getAllPagingPostServiceQuery;
            _getPostServiceByIdQuery = getPostServiceByIdQuery;
            _registerServiceFromProviderCommand = registerServiceFromProviderCommand;
            _registerServiceFromUserCommand = registerServiceFromUserCommand;
            _rejectPostServiceCommand = rejectPostServiceCommand;
            _updatePostServiceCommand = updatePostServiceCommand;
        }
        #region GETAPI
        [HttpGet("getAllPagingPostService")]
        public async Task<IActionResult> GetAllPagingPostService(string keyword, int page, int pageSize)
        {
            var model = await _getAllPagingPostServiceQuery.ExecuteAsync(keyword, page, pageSize);
            return new OkObjectResult(model);
        }

        [HttpGet("getPostServiceById")]
        public async Task<IActionResult> GetPostServiceById(Guid idService)
        {
            var model = await _getPostServiceByIdQuery.ExecuteAsync(idService);
            return new OkObjectResult(model);
        }
        #endregion

        #region DeleteAPI 
        [HttpDelete("deleteServiceFromProvider")]
        public async Task<IActionResult> DeleteServiceFromProvider(Guid idService)
        {
            var model = await _deleteServiceFromProviderCommand.ExecuteAsync(idService);
            return new OkObjectResult(model);
        }

        [HttpDelete("deleteServiceFromUser")]
        public async Task<IActionResult> DeleteServiceFromUser(Guid idService)
        {
            var model = await _deleteServiceFromUserCommand.ExecuteAsync(idService);
            return new OkObjectResult(model);
        }
        #endregion

        #region  Post API
        [HttpPost("approvePostService")]
        public async Task<IActionResult> ApprovePostService([FromBody]PostServiceViewModel vm)
        {
            var model = await _approvePostServiceCommand.ExecuteAsync(vm);
            return new OkObjectResult(model);
        }

        [HttpPost("rejectPostService")]
        public async Task<IActionResult> RejectPostService([FromBody]PostServiceViewModel vm)
        {
            var model = await _rejectPostServiceCommand.ExecuteAsync(vm);
            return new OkObjectResult(model);
        }

        [HttpPost("registerServiceFromProvider")]
        public async Task<IActionResult> RegisterServiceFromProvider([FromBody]PostServiceViewModel vm)
        {
            var model = await _registerServiceFromProviderCommand.ExecuteAsync(vm);
            return new OkObjectResult(model);
        }

        [HttpPost("registerServiceFromUser")]
        public async Task<IActionResult> RegisterServiceFromUser([FromBody]PostServiceViewModel vm)
        {
            var model = await _registerServiceFromUserCommand.ExecuteAsync(vm);
            return new OkObjectResult(model);
        }
        #endregion

        #region PUT API
        [HttpPost("updatePostService")]
        public async Task<IActionResult> updatePostService([FromBody]PostServiceViewModel vm)
        {
            var model = await _updatePostServiceCommand.ExecuteAsync(vm);
            return new OkObjectResult(model);
        }
        #endregion

    }
}