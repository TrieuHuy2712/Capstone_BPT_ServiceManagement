using BPT_Service.Application.Interfaces;
using BPT_Service.Application.ViewModels.System;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BPT_Service.WebAPI.Controllers
{
    [Authorize]
    [Route("CategoryManagement")]
    public class CategoryController: ControllerBase
    {
        private readonly ICategoryService _categoryService;
        public CategoryController(ICategoryService categoryService)
        {
            _categoryService = categoryService;
        }
        #region GET API
        [HttpGet("GetAllCategory")]
        public IActionResult GetAllCategory()
        {
            var model = _categoryService.GetAllAsync();
            return new OkObjectResult(model);
        }

        [HttpGet("GetCatagoryById")]
        public IActionResult GetAllFillter(int id)
        {
            var model = _categoryService.GetByID(id);
            return new OkObjectResult(model);
        }

        [HttpGet("GetAllPaging")]
        public IActionResult GetAllPaging(string keyword, int page, int pageSize)
        {
            var model = _categoryService.GetAllPagingAsync(keyword, page, pageSize);
            return new OkObjectResult(model);
        }
        #endregion

        #region POST API
        [HttpPost("addNewCategory")]
        public IActionResult AddNewCategory([FromBody]CategoryViewModel categoryVm)
        {
            if (!ModelState.IsValid)
            {
                IEnumerable<ModelError> allErrors = ModelState.Values.SelectMany(v => v.Errors);
                return new BadRequestObjectResult(allErrors);
            }
            else
            {
                _categoryService.AddAsync(categoryVm);
                _categoryService.Save();
                return new OkObjectResult(categoryVm);
            }
        }
        #endregion

        #region PUT API
        [HttpPut("updateCategory")]
        public IActionResult UpdateCategory([FromBody]CategoryViewModel categoryVm)
        {
            if (!ModelState.IsValid)
            {
                IEnumerable<ModelError> allErrors = ModelState.Values.SelectMany(v => v.Errors);
                return new BadRequestObjectResult(allErrors);
            }
            else
            {
                _categoryService.Update(categoryVm);

                _categoryService.Save();
                return new OkObjectResult(categoryVm);
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
                await _categoryService.DeleteAsync(id);
                _categoryService.Save();
                return new OkObjectResult(id);
            }
        }
        #endregion

    }
}
