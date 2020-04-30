using BPT_Service.Application.PostService.Command.ApprovePostService;
using BPT_Service.Application.PostService.Command.ConfirmPostService;
using BPT_Service.Application.PostService.Command.PostServiceFromProvider.DeleteServiceFromProvider;
using BPT_Service.Application.PostService.Command.PostServiceFromProvider.RegisterServiceFromProvider;
using BPT_Service.Application.PostService.Command.PostServiceFromUser.DeleteServiceFromUser;
using BPT_Service.Application.PostService.Command.PostServiceFromUser.RegisterServiceFromUser;
using BPT_Service.Application.PostService.Command.RejectPostService;
using BPT_Service.Application.PostService.Command.UpdatePostService;
using BPT_Service.Application.PostService.Query.FilterAllPagingPostService;
using BPT_Service.Application.PostService.Query.GetAllPagingPostService;
using BPT_Service.Application.PostService.Query.GetAllPostUserServiceByUserId;
using BPT_Service.Application.PostService.Query.GetPostServiceById;
using BPT_Service.Application.PostService.ViewModel;
using BPT_Service.WebAPI.Models.ServiceViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace BPT_Service.WebAPI.Controllers
{
    [Authorize]
    [ApiController]
    [Route("Service")]
    public class ServiceController : ControllerBase
    {
        private readonly IApprovePostServiceCommand _approvePostServiceCommand;
        private readonly IConfirmPostService _confirmPostService;
        private readonly IDeleteServiceFromProviderCommand _deleteServiceFromProviderCommand;
        private readonly IDeleteServiceFromUserCommand _deleteServiceFromUserCommand;
        private readonly IFilterAllPagingPostServiceQuery _filterAllPagingPostServiceQuery;
        private readonly IGetAllPagingPostServiceQuery _getAllPagingPostServiceQuery;
        private readonly IGetAllPostUserServiceByUserIdQuery _getAllPostUserServiceByUserIdQuery;
        private readonly IGetPostServiceByIdQuery _getPostServiceByIdQuery;
        private readonly IRegisterServiceFromProviderCommand _registerServiceFromProviderCommand;
        private readonly IRegisterServiceFromUserCommand _registerServiceFromUserCommand;
        private readonly IRejectPostServiceCommand _rejectPostServiceCommand;
        private readonly IUpdatePostServiceCommand _updatePostServiceCommand;

        public ServiceController(
            IApprovePostServiceCommand approvePostServiceCommand, 
            IConfirmPostService confirmPostService, 
            IDeleteServiceFromProviderCommand deleteServiceFromProviderCommand, 
            IDeleteServiceFromUserCommand deleteServiceFromUserCommand, 
            IFilterAllPagingPostServiceQuery filterAllPagingPostServiceQuery, 
            IGetAllPagingPostServiceQuery getAllPagingPostServiceQuery, 
            IGetAllPostUserServiceByUserIdQuery getAllPostUserServiceByUserIdQuery, 
            IGetPostServiceByIdQuery getPostServiceByIdQuery, 
            IRegisterServiceFromProviderCommand registerServiceFromProviderCommand, 
            IRegisterServiceFromUserCommand registerServiceFromUserCommand, 
            IRejectPostServiceCommand rejectPostServiceCommand, 
            IUpdatePostServiceCommand updatePostServiceCommand)
        {
            _approvePostServiceCommand = approvePostServiceCommand;
            _confirmPostService = confirmPostService;
            _deleteServiceFromProviderCommand = deleteServiceFromProviderCommand;
            _deleteServiceFromUserCommand = deleteServiceFromUserCommand;
            _filterAllPagingPostServiceQuery = filterAllPagingPostServiceQuery;
            _getAllPagingPostServiceQuery = getAllPagingPostServiceQuery;
            _getAllPostUserServiceByUserIdQuery = getAllPostUserServiceByUserIdQuery;
            _getPostServiceByIdQuery = getPostServiceByIdQuery;
            _registerServiceFromProviderCommand = registerServiceFromProviderCommand;
            _registerServiceFromUserCommand = registerServiceFromUserCommand;
            _rejectPostServiceCommand = rejectPostServiceCommand;
            _updatePostServiceCommand = updatePostServiceCommand;
        }


        #region GETAPI

        [HttpGet("getAllPagingPostService")]
        public async Task<IActionResult> GetAllPagingPostService(string keyword, int page, int pageSize, bool isAdminPage,int filter)
        {
            var model = await _getAllPagingPostServiceQuery.ExecuteAsync(keyword, page, pageSize, isAdminPage,filter);
            return new OkObjectResult(model);
        }

        [HttpGet("getAllPostUserServiceByUserId")]
        public async Task<IActionResult> GetAllPostUserServiceByUserId(string idUser)
        {
            var model = await _getAllPostUserServiceByUserIdQuery.ExecuteAsync(idUser);
            return new OkObjectResult(model);
        }

        [HttpGet("confirmPostService/{codeOTP}")]
        public async Task<IActionResult> ConfirmPostService(string codeOTP)
        {
            var model = await _confirmPostService.ExecuteAsync(codeOTP);
            return new OkObjectResult(model);
        }

        [HttpGet("getPostServiceById")]
        public async Task<IActionResult> GetPostServiceById(string idService)
        {
            var model = await _getPostServiceByIdQuery.ExecuteAsync(idService);
            return new OkObjectResult(model);
        }

        [HttpGet("getFilterAllPaging")]
        public async Task<IActionResult> GetFilterAllPaging(int page, int pageSize, string typeFilter, string filterName)
        {
            var model = await _filterAllPagingPostServiceQuery.ExecuteAsync(page, pageSize, typeFilter, filterName);
            return new OkObjectResult(model);
        }

        #endregion GETAPI

        #region DeleteAPI

        [HttpDelete("deleteServiceFromProvider")]
        public async Task<IActionResult> DeleteServiceFromProvider(string id)
        {
            var model = await _deleteServiceFromProviderCommand.ExecuteAsync(id);
            return new OkObjectResult(model);
        }

        [HttpDelete("deleteServiceFromUser")]
        public async Task<IActionResult> DeleteServiceFromUser(string id)
        {
            var model = await _deleteServiceFromUserCommand.ExecuteAsync(id);
            return new OkObjectResult(model);
        }

        #endregion DeleteAPI

        #region Post API

        [HttpPost("approvePostService")]
        public async Task<IActionResult> ApprovePostService([FromBody]PostServiceViewModel vm)
        {
            var model = await _approvePostServiceCommand.ExecuteAsync(vm.Id);
            return new OkObjectResult(model);
        }

        [HttpPost("rejectPostService")]
        public async Task<IActionResult> RejectPostService([FromBody]PostServiceViewModel vm)
        {
            var model = await _rejectPostServiceCommand.ExecuteAsync(vm.Id, vm.Reason);
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

        #endregion Post API

        #region PUT API

        [HttpPost("updatePostService")]
        public async Task<IActionResult> updatePostService([FromBody]PostServiceViewModel vm)
        {
            var model = await _updatePostServiceCommand.ExecuteAsync(vm);
            return new OkObjectResult(model);
        }

        #endregion PUT API
    }
}