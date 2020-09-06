using BPT_Service.Application.LocationService.Command.AddCityProvinceService;
using BPT_Service.Application.LocationService.Command.DeleteCityProvinceService;
using BPT_Service.Application.LocationService.Command.UpdateCityProvinceService;
using BPT_Service.Application.LocationService.Query.GetAllCityProvinceService;
using BPT_Service.Application.LocationService.Query.GetAllPagingCityProvinceService;
using BPT_Service.Application.LocationService.Query.GetByIdCityProvinceService;
using BPT_Service.Application.LocationService.ViewModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BPT_Service.WebAPI.Controllers
{
    [Authorize]
    [Route("LocationManagement")]
    public class LocationController : ControllerBase
    {
        private readonly IAddCityProvinceServiceCommand _addCityProvinceServiceCommand;
        private readonly IDeleteCityProvinceServiceCommand _deleteCityProvinceService;
        private readonly IUpdateCityProvinceServiceCommand _updateCityProvinceService;
        private readonly IGetAllCityProvinceServiceQuery _getAllCityProvinceServiceQuery;
        private readonly IGetAllPagingCityProvinceServiceQuery _getAllPagingCityProvinceServiceQuery;
        private readonly IGetByIdCityProvinceServiceQuery _getByIdCityProvinceService;

        public LocationController(IAddCityProvinceServiceCommand addCityProvinceServiceCommand,
            IDeleteCityProvinceServiceCommand deleteCityProvinceService,
            IUpdateCityProvinceServiceCommand updateCityProvinceService,
            IGetAllCityProvinceServiceQuery getAllCityProvinceServiceQuery,
            IGetAllPagingCityProvinceServiceQuery getAllPagingCityProvinceServiceQuery,
            IGetByIdCityProvinceServiceQuery getByIdCityProvinceService)
        {
            _addCityProvinceServiceCommand = addCityProvinceServiceCommand;
            _deleteCityProvinceService = deleteCityProvinceService;
            _updateCityProvinceService = updateCityProvinceService;
            _getAllCityProvinceServiceQuery = getAllCityProvinceServiceQuery;
            _getAllPagingCityProvinceServiceQuery = getAllPagingCityProvinceServiceQuery;
            _getByIdCityProvinceService = getByIdCityProvinceService;
        }
        #region GET API
        [AllowAnonymous]
        [HttpGet("GetAllLocation")]
        public async Task<IActionResult> GetAllCategory()
        {
            var model = await _getAllCityProvinceServiceQuery.ExecuteAsync();
            return new OkObjectResult(model);
        }

        [HttpGet("GetLocationById")]
        public async Task<IActionResult> GetAllFillter(int id)
        {
            var model = await _getByIdCityProvinceService.ExecuteAsync(id);
            return new OkObjectResult(model);
        }

        [HttpGet("GetAllPaging")]
        public async Task<IActionResult> GetAllPaging(string keyword, int page, int pageSize)
        {
            var model = await _getAllPagingCityProvinceServiceQuery.ExecuteAsync(keyword, page, pageSize);
            return new OkObjectResult(model);
        }
        #endregion

        #region POST API
        [HttpPost("addNewLocation")]
        public async Task<IActionResult> AddNewCategory([FromBody]CityProvinceViewModel vm)
        {
            if (!ModelState.IsValid)
            {
                IEnumerable<ModelError> allErrors = ModelState.Values.SelectMany(v => v.Errors);
                return new BadRequestObjectResult(allErrors);
            }
            else
            {
                var execute = await _addCityProvinceServiceCommand.ExecuteAsync(vm);
                return new OkObjectResult(execute);
            }
        }
        #endregion

        #region PUT API
        [HttpPut("updateLocation")]
        public async Task<IActionResult> UpdateCategory([FromBody]CityProvinceViewModel locationVm)
        {
            if (!ModelState.IsValid)
            {
                IEnumerable<ModelError> allErrors = ModelState.Values.SelectMany(v => v.Errors);
                return new BadRequestObjectResult(allErrors);
            }
            else
            {
                var execute = await _updateCityProvinceService.ExecuteAsync(locationVm);
                return new OkObjectResult(execute);
            }
        }
        #endregion

        #region DELETE API
        [HttpDelete("DeleteCategory")]
        public async Task<IActionResult> Delete(int id)
        {
            if (!ModelState.IsValid)
            {
                return new BadRequestResult();
            }
            else
            {
                var execute = await _deleteCityProvinceService.ExecuteAsync(id);
                return new OkObjectResult(execute);
            }
        }
        #endregion

    }
}