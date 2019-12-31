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
    [Route("TagManagement")]
    public class TagController : ControllerBase
    {
        #region Constructor
        private readonly ITagService _tagService;
        public TagController(ITagService tagService)
        {
            _tagService = tagService;
        }
        #endregion

        #region GET API
        [HttpGet("GetAllTag")]
        public IActionResult GetAllTag()
        {
            var model = _tagService.GetAllAsync();
            return new OkObjectResult(model);
        }

        [HttpGet("GetTagById")]
        public IActionResult GetAllFillter(Guid id)
        {
            var model = _tagService.GetByID(id);
            return new OkObjectResult(model);
        }

        [HttpGet("GetAllPaging")]
        public IActionResult GetAllPaging(string keyword, int page, int pageSize)
        {
            var model = _tagService.GetAllPagingAsync(keyword, page, pageSize);
            return new OkObjectResult(model);
        }
        #endregion

        #region POST
        [HttpPost("addNewTag")]
        public IActionResult AddNewTag([FromBody]TagViewModel tagVm)
        {
            if (!ModelState.IsValid)
            {
                IEnumerable<ModelError> allErrors = ModelState.Values.SelectMany(v => v.Errors);
                return new BadRequestObjectResult(allErrors);
            }
            else
            {
                _tagService.AddAsync(tagVm);
                _tagService.Save();
                return new OkObjectResult(tagVm);
            }
        }
        #endregion

        #region PUT API
        [HttpPut("updateTag")]
        public IActionResult UpdateTag([FromBody]TagViewModel tagVM)
        {
            if (!ModelState.IsValid)
            {
                IEnumerable<ModelError> allErrors = ModelState.Values.SelectMany(v => v.Errors);
                return new BadRequestObjectResult(allErrors);
            }
            else
            {
                _tagService.Update(tagVM);

                _tagService.Save();
                return new OkObjectResult(tagVM);
            }
        }
        #endregion

        #region DELETE API
        [HttpDelete("DeleteTag")]
        public async Task<IActionResult> Delete(Guid id)
        {
            if (!ModelState.IsValid)
            {
                return new BadRequestResult();
            }
            else
            {
                await _tagService.DeleteAsync(id);
                _tagService.Save();
                return new OkObjectResult(id);
            }
        }
        #endregion

    }
}
