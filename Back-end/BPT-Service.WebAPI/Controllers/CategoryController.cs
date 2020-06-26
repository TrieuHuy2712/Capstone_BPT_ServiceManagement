using BPT_Service.Application.CategoryService.Command.AddCategoryService;
using BPT_Service.Application.CategoryService.Command.DeleteCategoryService;
using BPT_Service.Application.CategoryService.Command.UpdateCategoryService;
using BPT_Service.Application.CategoryService.Query.GetAllAsyncCategoryService;
using BPT_Service.Application.CategoryService.Query.GetAllPagingAsyncCategoryService;
using BPT_Service.Application.CategoryService.Query.GetByIDCategoryService;
using BPT_Service.Application.CategoryService.ViewModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BPT_Service.WebAPI.Controllers
{
    [Authorize]
    [Route("CategoryManagement")]
    public class CategoryController : ControllerBase
    {
        private readonly IAddCategoryServiceCommand _addCategoryService;
        private readonly IDeleteCategoryServiceCommand _deleteCategoryService;
        private readonly IGetAllAsyncCategoryServiceQuery _getAllCategoryService;
        private readonly IGetAllPagingAsyncCategoryServiceQuery _getAllPagingCategoryService;
        private readonly IGetByIDCategoryServiceQuery _getByIdCategoryService;
        private readonly IUpdateCategoryServiceCommand _updateCategoryService;

        public CategoryController(IAddCategoryServiceCommand addCategoryService,
        IDeleteCategoryServiceCommand deleteCategoryService,
        IGetAllAsyncCategoryServiceQuery getAllCategoryService,
        IGetAllPagingAsyncCategoryServiceQuery getAllPagingCategoryService,
        IGetByIDCategoryServiceQuery getByIdCategoryService,
        IUpdateCategoryServiceCommand updateCategoryService)
        {
            _addCategoryService = addCategoryService;
            _deleteCategoryService = deleteCategoryService;
            _getAllCategoryService = getAllCategoryService;
            _getAllPagingCategoryService = getAllPagingCategoryService;
            _getByIdCategoryService = getByIdCategoryService;
            _updateCategoryService = updateCategoryService;
        }
        #region GET API
        [AllowAnonymous]
        [HttpGet("GetAllCategory")]
        public async Task<IActionResult> GetAllCategory()
        {
            var model = await _getAllCategoryService.ExecuteAsync();
            return new OkObjectResult(model);
        }

        [HttpGet("GetCatagoryById")]
        public async Task<IActionResult> GetAllFillter(int id)
        {
            var model = await _getByIdCategoryService.ExecuteAsync(id);
            return new OkObjectResult(model);
        }

        [HttpGet("GetAllPaging")]
        public async Task<IActionResult> GetAllPaging(string keyword, int page, int pageSize)
        {
            var model = await _getAllPagingCategoryService.ExecuteAsync(keyword, page, pageSize);
            return new OkObjectResult(model);
        }
        #endregion

        #region POST API
        [HttpPost("addNewCategory")]
        public async Task<IActionResult> AddNewCategory([FromBody]CategoryServiceViewModel categoryVm)
        {
            if (!ModelState.IsValid)
            {
                IEnumerable<ModelError> allErrors = ModelState.Values.SelectMany(v => v.Errors);
                return new BadRequestObjectResult(allErrors);
            }
            else
            {
                var execute = await _addCategoryService.ExecuteAsync(categoryVm);
                return new OkObjectResult(execute);
            }
        }
        #endregion

        #region PUT API
        [HttpPut("updateCategory")]
        public async Task<IActionResult> UpdateCategory([FromBody]CategoryServiceViewModel categoryVm)
        {
            if (!ModelState.IsValid)
            {
                IEnumerable<ModelError> allErrors = ModelState.Values.SelectMany(v => v.Errors);
                return new BadRequestObjectResult(allErrors);
            }
            else
            {
                var execute = await _updateCategoryService.ExecuteAsync(categoryVm);
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
                var execute = await _deleteCategoryService.ExecuteAsync(id);
                return new OkObjectResult(execute);
            }
        }
        #endregion

    }
}
