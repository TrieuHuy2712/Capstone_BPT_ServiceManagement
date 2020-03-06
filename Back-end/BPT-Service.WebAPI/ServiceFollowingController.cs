using BPT_Service.Application.FollowingPostService.Command.FollowPostService;
using BPT_Service.Application.FollowingPostService.Command.UnFollowPostService;
using BPT_Service.Application.FollowingPostService.Query.GetFollowByPost;
using BPT_Service.Application.FollowingPostService.Query.GetFollowByUser;
using BPT_Service.Application.FollowingPostService.ViewModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BPT_Service.WebAPI
{
    [Authorize]
    [ApiController]
    [Route("ServiceFollowing")]
    public class ServiceFollowingController : ControllerBase
    {
        private readonly IFollowPostServiceCommand _followPostServiceCommand;
        private readonly IUnFollowPostServiceCommand _unFollowPostServiceCommand;
        private readonly IGetFollowByPostQuery _getFollowByPostQuery;
        private readonly IGetFollowByUserQuery _getFollowByUserQuery;
        public ServiceFollowingController(IFollowPostServiceCommand followPostServiceCommand,
            IUnFollowPostServiceCommand unFollowPostServiceCommand,
            IGetFollowByPostQuery getFollowByPostQuery,
            IGetFollowByUserQuery getFollowByUserQuery)
        {
            _followPostServiceCommand = followPostServiceCommand;
            _unFollowPostServiceCommand = unFollowPostServiceCommand;
            _getFollowByPostQuery = getFollowByPostQuery;
            _getFollowByUserQuery = getFollowByUserQuery;

        }
        #region GET API
        

        [HttpGet("GetServiceFollow")]
        public async Task<IActionResult> GetServiceFollow(string idService)
        {
            var model = await _getFollowByPostQuery.ExecuteAsync(idService);

            return new OkObjectResult(model);
        }

        [HttpGet("GetUserFollow")]
        public async Task<IActionResult> GetUserFollow(string idUser)
        {
            var model = await _getFollowByUserQuery.ExecuteAsync(idUser);

            return new OkObjectResult(model);
        }
        #endregion

        #region POST API
        [HttpPost("FollowService")]
        public async Task<IActionResult> FollowService(ServiceFollowingViewModel vm)
        {
            var model = await _followPostServiceCommand.ExecuteAsync(vm);

            return new OkObjectResult(model);
        }

        [HttpPost("UnFollowService")]
        public async Task<IActionResult> UnFollowService(ServiceFollowingViewModel vm)
        {
            var model = await _unFollowPostServiceCommand.ExecuteAsync(vm);

            return new OkObjectResult(model);
        }
        #endregion
    }
}
