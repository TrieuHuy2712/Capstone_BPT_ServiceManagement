using BPT_Service.Application.CommentService.Command.AddCommentServiceAsync;
using BPT_Service.Application.CommentService.Command.DeleteCommentServiceAsync;
using BPT_Service.Application.CommentService.Command.UpdateCommentServiceAsync;
using BPT_Service.Application.CommentService.Query.GetCommentServiceByIDAsync;
using BPT_Service.Application.CommentService.ViewModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.Collections.Generic;
using System.Linq;
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
        private readonly IUpdateCommentServiceAsyncCommand _updateCommentService;
        private readonly IDeleteCommentServiceAsyncCommand _deleteCommentService;

        public CommentController(
        IGetCommentServiceByIDAsyncQuery getCommentServiceByID,
        IAddCommentServiceAsyncCommand addCommentService,
        IUpdateCommentServiceAsyncCommand updateCommentService,
        IDeleteCommentServiceAsyncCommand deleteCommentService)
        {
            _getCommentServiceByID = getCommentServiceByID;
            _addCommentService = addCommentService;
            _updateCommentService = updateCommentService;
            _deleteCommentService = deleteCommentService;
        }

        #endregion Constructor

        #region GET API

        [AllowAnonymous]
        [HttpGet("getComment")]
        public async Task<IActionResult> GetByID(string id)
        {
            var model = await _getCommentServiceByID.ExecuteAsync(id);
            return new OkObjectResult(model);
        }

        #endregion GET API

        #region POST

        [HttpPost("addNewComment")]
        public async Task<IActionResult> AddNewComment([FromBody]CommentViewModel commentVm)
        {
            var execute = await _addCommentService.ExecuteAsync(commentVm);
            return new OkObjectResult(execute);
        }

        #endregion POST

        #region PUT API

        [HttpPut("updateComment")]
        public async Task<IActionResult> UpdateComment([FromBody]CommentViewModel commentserviceVm)
        {
            if (!ModelState.IsValid)
            {
                IEnumerable<ModelError> allErrors = ModelState.Values.SelectMany(v => v.Errors);
                return new BadRequestObjectResult(allErrors);
            }
            else
            {
                var execute = await _updateCommentService.ExecuteAsync(commentserviceVm);
                return new OkObjectResult(execute);
            }
        }

        #endregion PUT API

        #region DELETE API

        [HttpDelete("DeleteComment")]
        public async Task<IActionResult> Delete(int id)
        {
            if (!ModelState.IsValid)
            {
                return new BadRequestResult();
            }
            else
            {
                var execute = await _deleteCommentService.ExecuteAsync(id);
                return new OkObjectResult(execute);
            }
        }

        #endregion DELETE API
    }
}