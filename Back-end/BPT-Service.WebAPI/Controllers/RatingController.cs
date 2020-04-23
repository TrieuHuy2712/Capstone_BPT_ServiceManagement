using BPT_Service.Application.RatingService.Command.AddRatingService;
using BPT_Service.Application.RatingService.Command.DeleteRatingService;
using BPT_Service.Application.RatingService.Query.GetAllPagingRatingServiceByOwner;
using BPT_Service.Application.RatingService.Query.GetAllServiceRatingByUser;
using BPT_Service.Application.RatingService.Query.GetListAllPagingRatingService;
using BPT_Service.Application.RatingService.ViewModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace BPT_Service.WebAPI.Controllers
{
    [Authorize]
    [Route("RatingService")]
    public class RatingController : ControllerBase
    {
        private readonly IAddUpdateRatingServiceCommand _addUpdateRatingServiceCommand;
        private readonly IDeleteRatingServiceCommand _deleteRatingServiceCommand;
        private readonly IGetAllPagingRatingServiceByOwnerQuery _getAllPagingRatingServiceByOwnerQuery;
        private readonly IGetAllServiceRatingByUserQuery _getAllServiceRatingByUserQuery;
        private readonly IGetListAllPagingRatingServiceQuery _getListAllPagingRatingServiceQuery;

        public RatingController(
            IAddUpdateRatingServiceCommand addUpdateRatingServiceCommand,
        IDeleteRatingServiceCommand deleteRatingServiceCommand,
        IGetAllPagingRatingServiceByOwnerQuery getAllPagingRatingServiceByOwnerQuery,
        IGetAllServiceRatingByUserQuery getAllServiceRatingByUserQuery,
        IGetListAllPagingRatingServiceQuery getListAllPagingRatingServiceQuery)
        {
            _addUpdateRatingServiceCommand = addUpdateRatingServiceCommand;
            _deleteRatingServiceCommand = deleteRatingServiceCommand;
            _getAllPagingRatingServiceByOwnerQuery = getAllPagingRatingServiceByOwnerQuery;
            _getAllServiceRatingByUserQuery = getAllServiceRatingByUserQuery;
            _getListAllPagingRatingServiceQuery = getListAllPagingRatingServiceQuery;
        }

        [HttpGet("GetDetailARatingForProvider")]
        public async Task<IActionResult> GetDetailARatingForProvider(string keyword, int page, int pageSize, string idService)
        {
            var model = await _getAllPagingRatingServiceByOwnerQuery.ExecuteAsync(keyword, page, pageSize, idService);
            return new OkObjectResult(model);
        }

        [HttpGet("GetListRatingOfProvider")]
        public async Task<IActionResult> GetListRatingForProvider(string keyword, int page, int pageSize)
        {
            var model = await _getAllServiceRatingByUserQuery.ExecuteAsync(keyword, page, pageSize);
            return new OkObjectResult(model);
        }

        [HttpGet("GetAllListRating")]
        public async Task<IActionResult> GetAllListRating(string keyword, int page, int pageSize)
        {
            var model = await _getListAllPagingRatingServiceQuery.ExecuteAsync(keyword, page, pageSize);
            return new OkObjectResult(model);
        }

        [HttpPost("AddUpdateRating")]
        public async Task<IActionResult> AddUpdateRating(ServiceRatingViewModel vm)
        {
            var model = await _addUpdateRatingServiceCommand.ExecuteAsync(vm);
            return new OkObjectResult(model);
        }

        [HttpDelete("DeleteRating")]
        public async Task<IActionResult> DeleteRating(int idRating)
        {
            var model = await _deleteRatingServiceCommand.ExecuteAsync(idRating);
            return new OkObjectResult(model);
        }
    }
}