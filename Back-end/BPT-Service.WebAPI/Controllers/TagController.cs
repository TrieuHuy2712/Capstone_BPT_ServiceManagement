using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System;
using BPT_Service.Application.TagService.Command.AddServiceAsync;
using BPT_Service.Application.TagService.Command.DeleteServiceAsync;
using BPT_Service.Application.TagService.Command.UpdateTagServiceAsync;
using BPT_Service.Application.TagService.Query.GetAllPagingServiceAsync;
using BPT_Service.Application.TagService.Query.GetAllServiceAsync;
using BPT_Service.Application.TagService.Query.GetByIDTagServiceAsync;
using BPT_Service.Application.TagService.ViewModel;

namespace BPT_Service.WebAPI.Controllers
{
    [Authorize]
    [Route("TagManagement")]
    public class TagController : ControllerBase
    {
        #region Constructor
        private readonly IAddTagServiceAsyncCommand _addTagService;
        private readonly IDeleteTagServiceAsyncCommand _deleteTagService;
        private readonly IUpdateTagServiceAsyncCommand _updateTagService;
        private readonly IGetAllPagingTagServiceAsyncQuery _getAllPagingTagService;
        private readonly IGetAllTagServiceAsyncQuery _getAllTagService;
        private readonly IGetByIDTagServiceAsyncQuery _getByIdTagService;
        public TagController(IAddTagServiceAsyncCommand addTagService,
        IUpdateTagServiceAsyncCommand updateTagService,
        IDeleteTagServiceAsyncCommand deleteTagService,
        IGetAllPagingTagServiceAsyncQuery getAllPagingTagService,
        IGetAllTagServiceAsyncQuery getAllTagService,
        IGetByIDTagServiceAsyncQuery getByIdTagService)
        {
            _addTagService = addTagService;
            _deleteTagService = deleteTagService;
            _updateTagService = updateTagService;
            _getAllPagingTagService = getAllPagingTagService;
            _getAllTagService = getAllTagService;
            _getByIdTagService = getByIdTagService;
        }
        #endregion

        #region GET API
        [HttpGet("GetAllTag")]
        public async Task<IActionResult> GetAllTag()
        {
            var model = _getAllTagService.ExecuteAsync();
            return new OkObjectResult(model);
        }

        [HttpGet("GetTagById")]
        public async Task<IActionResult> GetAllFillter(Guid id)
        {
            var model = _getByIdTagService.ExecuteAsync(id);
            return new OkObjectResult(model);
        }

        [HttpGet("GetAllPaging")]
        public async Task<IActionResult> GetAllPaging(string keyword, int page, int pageSize)
        {
            var model = await _getAllPagingTagService.ExecuteAsync(keyword, page, pageSize);
            return new OkObjectResult(model);
        }
        #endregion

        #region POST
        [HttpPost("addNewTag")]
        public async Task<IActionResult> AddNewTag([FromBody]TagViewModel tagVm)
        {
            if (!ModelState.IsValid)
            {
                IEnumerable<ModelError> allErrors = ModelState.Values.SelectMany(v => v.Errors);
                return new BadRequestObjectResult(allErrors);
            }
            else
            {
                var execute = await _addTagService.ExecuteAsync(tagVm);
                return new OkObjectResult(execute);
            }
        }
        #endregion

        #region PUT API
        [HttpPut("updateTag")]
        public async Task<IActionResult> UpdateTag([FromBody]TagViewModel tagVM)
        {
            if (!ModelState.IsValid)
            {
                IEnumerable<ModelError> allErrors = ModelState.Values.SelectMany(v => v.Errors);
                return new BadRequestObjectResult(allErrors);
            }
            else
            {
                var execute = await _updateTagService.ExecuteAsync(tagVM);
                return new OkObjectResult(execute);
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
                var execute = await _deleteTagService.ExecuteAsync(id);
                return new OkObjectResult(execute);
            }
        }
        #endregion

    }
}
