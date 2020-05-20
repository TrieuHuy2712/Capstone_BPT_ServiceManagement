using BPT_Service.Application.RecommedationService.Command.AddRecommendService;
using BPT_Service.Application.RecommedationService.Command.RecommendLocation.AddRecommendLocation;
using BPT_Service.Application.RecommedationService.Command.RecommendLocation.DeleteRecommendLocation;
using BPT_Service.Application.RecommedationService.Command.RecommendNews.AddRecommendNews;
using BPT_Service.Application.RecommedationService.Query.GetRecommendByLocation;
using BPT_Service.Application.RecommedationService.Query.GetRecommendByNews;
using BPT_Service.Application.RecommedationService.Query.GetRecommendByService;
using BPT_Service.Application.RecommedationService.ViewModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace BPT_Service.WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [AllowAnonymous]
    public class RecommendationController : ControllerBase
    {
        private readonly IGetRecommendByLocation _getRecommendByLocation;
        private readonly IGetRecommendByNews _getRecommendByNews;
        private readonly IDeleteRecommend _deleteRecommend;
        private readonly IAddRecommendLocation _addLocationRecommend;
        private readonly IAddRecommendNews _addNewsRecommend;
        private readonly IAddRecommendService _addRecommendService;
        private readonly IGetRecommendByService _getRecommendByService;

        public RecommendationController(
            IGetRecommendByLocation getRecommendByLocation,
            IGetRecommendByNews getRecommendByNews,
            IDeleteRecommend deleteRecommend,
            IAddRecommendLocation addLocationRecommend,
            IAddRecommendNews addNewsRecommend,
            IAddRecommendService addRecommendService,
            IGetRecommendByService getRecommendByService)
        {
            _getRecommendByLocation = getRecommendByLocation;
            _getRecommendByNews = getRecommendByNews;
            _deleteRecommend = deleteRecommend;
            _addLocationRecommend = addLocationRecommend;
            _addNewsRecommend = addNewsRecommend;
            _addRecommendService = addRecommendService;
            _getRecommendByService = getRecommendByService;
        }

        #region GET API

        [HttpGet("GetRecommendLocation")]
        public async Task<IActionResult> GetRecommendLocation(bool isDefault)
        {
            var model = await _getRecommendByLocation.ExecuteAsync(isDefault);
            return new OkObjectResult(model);
        }

        [HttpGet("GetRecommendNews")]
        public async Task<IActionResult> GetRecommendNews(bool isDefault)
        {
            var model = await _getRecommendByNews.ExecuteAsync(isDefault);
            return new OkObjectResult(model);
        }

        [HttpGet("GetRecommendService")]
        public async Task<IActionResult> GetRecommendService(bool isDefault)
        {
            var model = await _getRecommendByService.ExecuteAsync(isDefault);
            return new OkObjectResult(model);
        }

        #endregion GET API

        #region POST API

        [HttpPost("AddLocationRecommend")]
        public async Task<IActionResult> AddLocationRecommend(LocationRecommendationViewModel vm)
        {
            var model = await _addLocationRecommend.ExecuteAsync(vm);
            return new OkObjectResult(model);
        }

        [HttpPost("AddNewsRecommend")]
        public async Task<IActionResult> AddNewsRecommend(NewsRecommendationViewModel vm)
        {
            var model = await _addNewsRecommend.ExecuteAsync(vm);
            return new OkObjectResult(model);
        }

        [HttpPost("AddServiceRecommend")]
        public async Task<IActionResult> AddServiceRecommend(ServiceRecommendationViewModel vm)
        {
            var model = await _addRecommendService.ExecuteAsync(vm);
            return new OkObjectResult(model);
        }

        #endregion POST API

        #region DELETE API

        [HttpPost("DeleteRecommend")]
        public async Task<IActionResult> DeleteRecommend(int id)
        {
            var model = await _deleteRecommend.ExecuteAsync(id);
            return new OkObjectResult(model);
        }

        #endregion DELETE API
    }
}