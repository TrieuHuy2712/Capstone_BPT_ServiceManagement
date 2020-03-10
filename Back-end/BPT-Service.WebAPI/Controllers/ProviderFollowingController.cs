using BPT_Service.Application.FollowingPostService.Query.GetFollowByUser;
using BPT_Service.Application.FollowingProviderService.Command.FollowProviderService;
using BPT_Service.Application.FollowingProviderService.Command.RegisterEmailProviderService;
using BPT_Service.Application.FollowingProviderService.Command.UnFollowProviderService;
using BPT_Service.Application.FollowingProviderService.Query.GetFollowByProvider;
using BPT_Service.Application.FollowingProviderService.ViewModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace BPT_Service.WebAPI.Controllers
{
    [Authorize]
    [ApiController]
    [Route("ProviderFollowing")]
    public class ProviderFollowingController : ControllerBase
    {
        private readonly IFollowProviderServiceCommand _followProviderServiceCommand;
        private readonly IRegisterEmailProviderServiceCommand _registerEmailProviderServiceCommand;
        private readonly IUnFollowProviderServiceCommand _unFollowProviderServiceCommand;
        private readonly IGetFollowByProviderQuery _getFollowByProviderQuery;
        private readonly IGetFollowByUserQuery _getFollowByUserQuery;
        public ProviderFollowingController(IFollowProviderServiceCommand followProviderServiceCommand,
            IRegisterEmailProviderServiceCommand registerEmailProviderServiceCommand,
            IUnFollowProviderServiceCommand unFollowProviderServiceCommand,
            IGetFollowByProviderQuery getFollowByProviderQuery,
            IGetFollowByUserQuery getFollowByUserQuery)
        {
            _followProviderServiceCommand = followProviderServiceCommand;
            _registerEmailProviderServiceCommand = registerEmailProviderServiceCommand;
            _unFollowProviderServiceCommand = unFollowProviderServiceCommand;
            _getFollowByProviderQuery = getFollowByProviderQuery;
            _getFollowByUserQuery = getFollowByUserQuery;
        }

        #region GET API
        [HttpGet("GetListProviderFollow")]
        public async Task<IActionResult> GetListProviderFollow(string idProvider)
        {
            var model = await _getFollowByProviderQuery.ExecuteAsync(idProvider);

            return new OkObjectResult(model);
        }

        [HttpGet("GetListUserFollow")]
        public async Task<IActionResult> GetServiceFollow(string idUser)
        {
            var model = await _getFollowByUserQuery.ExecuteAsync(idUser);

            return new OkObjectResult(model);
        }
        #endregion

        #region POST API
        [HttpPost("FollowProviders")]
        public async Task<IActionResult> FollowProviders(FollowingProviderServiceViewModel vm)
        {
            var model = await _followProviderServiceCommand.ExecuteAsync(vm);
            return new OkObjectResult(model);
        }
        
        [HttpPost("RegisterEmailProviders")]
        public async Task<IActionResult> RegisterEmailProviders(int idRegister)
        {
            var model = await _registerEmailProviderServiceCommand.ExecuteAsync(idRegister);
            return new OkObjectResult(model);
        }

        [HttpPost("UnFollowProviders")]
        public async Task<IActionResult> UnFollowProviders(FollowingProviderServiceViewModel vm)
        {
            var model = await _unFollowProviderServiceCommand.ExecuteAsync(vm);
            return new OkObjectResult(model);
        }
        #endregion
    }
}
