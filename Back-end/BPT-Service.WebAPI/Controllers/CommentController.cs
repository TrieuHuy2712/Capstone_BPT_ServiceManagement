using BPT_Service.Application.CommentService.Command.AddCommentServiceAsync;
using BPT_Service.Application.CommentService.Query.GetCommentServiceByIDAsync;
using BPT_Service.Application.CommentService.ViewModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace BPT_Service.WebAPI.Controllers
{
    [Authorize]
    [Route("CommentManagement")]
    public class CommentController : ControllerBase
    {
        #region Constructor

        private readonly IGetCommentServiceByIDAsyncQuery _getCommentServiceByID;
        private readonly IAddCommentServiceAsyncCommand _addCommentService;
        public CommentController(
        IGetCommentServiceByIDAsyncQuery getCommentServiceByID,
        IAddCommentServiceAsyncCommand addCommentService)
        {
            _getCommentServiceByID = getCommentServiceByID;
            _addCommentService = addCommentService;
        }
        #endregion

        #region GET API

        [HttpGet("getComment")]
        public async Task<IActionResult> GetByID(string id)
        {
            var model = await _getCommentServiceByID.ExecuteAsync(id);
            return new OkObjectResult(model);
        }

        #endregion

        #region POST
        [HttpPost("addNewComment")]
        public async Task<IActionResult> AddNewComment(CommentViewModel commentVm)
        {
            
            var execute = await _addCommentService.ExecuteAsync(commentVm);
            return new OkObjectResult(execute);
        }
        #endregion

        // #region PUT API
        // [HttpPut("updateTag")]
        // public async Task<IActionResult> UpdateTag([FromBody]TagViewModel tagVM)
        // {
        //     if (!ModelState.IsValid)
        //     {
        //         IEnumerable<ModelError> allErrors = ModelState.Values.SelectMany(v => v.Errors);
        //         return new BadRequestObjectResult(allErrors);
        //     }
        //     else
        //     {
        //         var execute = await _updateTagService.ExecuteAsync(tagVM);
        //         return new OkObjectResult(execute);
        //     }
        // }
        // #endregion

        // #region DELETE API
        // [HttpDelete("DeleteTag")]
        // public async Task<IActionResult> Delete(Guid id)
        // {
        //     if (!ModelState.IsValid)
        //     {
        //         return new BadRequestResult();
        //     }
        //     else
        //     {
        //         var execute = await _deleteTagService.ExecuteAsync(id);
        //         return new OkObjectResult(execute);
        //     }
        // }
        // #endregion

    }
}