using BPT_Service.Application.RecommedationService.Command.AddRecommendService;
using BPT_Service.Application.RecommedationService.Command.RecommendLocation.AddRecommendLocation;
using BPT_Service.Application.RecommedationService.Command.RecommendLocation.DeleteRecommendLocation;
using BPT_Service.Application.RecommedationService.Command.RecommendNews.AddRecommendNews;
using BPT_Service.Application.RecommedationService.Command.ViewService;
using BPT_Service.Application.RecommedationService.Query.GetRecommendByLocation;
using BPT_Service.Application.RecommedationService.Query.GetRecommendByNews;
using BPT_Service.Application.RecommedationService.Query.GetRecommendByService;
using BPT_Service.Application.RecommedationService.Query.GetViewedService;
using BPT_Service.Application.RecommedationService.Query.RecommendUserService;
using BPT_Service.Application.RecommedationService.ViewModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace BPT_Service.WebAPI.Controllers
{
    [Route("Recommendation")]
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
        private readonly IViewUserService _viewUserService;
        private readonly IRecommendUserService _recommendUserService;
        private readonly IGetViewedServiceQuery _getViewedServiceQuery;

        public RecommendationController(
            IGetRecommendByLocation getRecommendByLocation,
            IGetRecommendByNews getRecommendByNews,
            IDeleteRecommend deleteRecommend,
            IAddRecommendLocation addLocationRecommend,
            IAddRecommendNews addNewsRecommend,
            IAddRecommendService addRecommendService,
            IGetRecommendByService getRecommendByService,
            IViewUserService viewUserService,
            IRecommendUserService recommendUserService,
            IGetViewedServiceQuery getViewedServiceQuery)
        {
            _getRecommendByLocation = getRecommendByLocation;
            _getRecommendByNews = getRecommendByNews;
            _deleteRecommend = deleteRecommend;
            _addLocationRecommend = addLocationRecommend;
            _addNewsRecommend = addNewsRecommend;
            _addRecommendService = addRecommendService;
            _getRecommendByService = getRecommendByService;
            _viewUserService = viewUserService;
            _recommendUserService = recommendUserService;
            _getViewedServiceQuery = getViewedServiceQuery;
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

        [HttpGet("GetRecommendUserService")]

        public async Task<IActionResult> GetRecommendUserService()
        {
            var model = await _recommendUserService.ExecuteAsync();
            return new OkObjectResult(model);
        }

        #endregion GET API

        #region POST API
        [HttpGet("ViewService")]
        public async Task<IActionResult> ViewService(string idService)
        {
            var model = await _viewUserService.ExecuteAsync(idService);
            return new OkObjectResult(model);
        }

        [HttpGet("HistoryService")]
        public async Task<IActionResult> HistoryService()
        {
            var model = await _getViewedServiceQuery.ExecuteAsync();
            return new OkObjectResult(model);
        }

        [HttpPost("AddLocationRecommend")]
        public async Task<IActionResult> AddLocationRecommend(AddRecommendationViewModel vm)
        {
            var model = await _addLocationRecommend.ExecuteAsync(vm);
            return new OkObjectResult(model);
        }

        [HttpPost("AddNewsRecommend")]
        public async Task<IActionResult> AddNewsRecommend(AddRecommendationViewModel vm)
        {
            var model = await _addNewsRecommend.ExecuteAsync(vm);
            return new OkObjectResult(model);
        }

        [HttpPost("AddServiceRecommend")]
        public async Task<IActionResult> AddServiceRecommend(AddRecommendationViewModel vm)
        {
            var model = await _addRecommendService.ExecuteAsync(vm);
            return new OkObjectResult(model);
        }

        #endregion POST API

        #region DELETE API

        [HttpDelete("DeleteRecommend")]
        public async Task<IActionResult> DeleteRecommend(int id)
        {
            var model = await _deleteRecommend.ExecuteAsync(id);
            return new OkObjectResult(model);
        }

        #endregion DELETE API
    }
}