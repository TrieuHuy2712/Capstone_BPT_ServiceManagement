using BPT_Service.Application.EmailService.Command.AddNewEmailService;
using BPT_Service.Application.EmailService.Command.DeleteEmailService;
using BPT_Service.Application.EmailService.Command.UpdateNewEmailService;
using BPT_Service.Application.EmailService.Query.GetAllEmailService;
using BPT_Service.Application.EmailService.Query.GetAllPagingEmailService;
using BPT_Service.Application.EmailService.Query.GetEmailByIdService;
using BPT_Service.Application.EmailService.ViewModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BPT_Service.WebAPI.Controllers
{
    [Authorize]
    [Route("EmailManagement")]
    public class EmailController : ControllerBase
    {
        private readonly IAddNewEmailServiceCommand _addNewEmailServiceCommand;
        private readonly IUpdateNewEmailServiceCommand _updateNewEmailServiceCommand;
        private readonly IDeleteEmailServiceCommand _deleteEmailServiceCommand;
        private readonly IGetAllEmailServiceQuery _getAllEmailServiceQuery;
        private readonly IGetAllPagingEmailServiceQuery _getAllPagingEmailService;
        private readonly IGetEmailByIdService _getEmailByIdService;

        public EmailController(IAddNewEmailServiceCommand addNewEmailServiceCommand,
            IUpdateNewEmailServiceCommand updateNewEmailServiceCommand,
            IDeleteEmailServiceCommand deleteEmailServiceCommand,
            IGetAllEmailServiceQuery getAllEmailServiceQuery,
            IGetAllPagingEmailServiceQuery getAllPagingEmailService,
            IGetEmailByIdService getEmailByIdService)
        {
            _addNewEmailServiceCommand = addNewEmailServiceCommand;
            _updateNewEmailServiceCommand = updateNewEmailServiceCommand;
            _deleteEmailServiceCommand = deleteEmailServiceCommand;
            _getAllEmailServiceQuery = getAllEmailServiceQuery;
            _getAllPagingEmailService = getAllPagingEmailService;
            _getEmailByIdService = getEmailByIdService;
        }
        
        #region GET API
        [HttpGet("GetAllEmail")]
        public async Task<IActionResult> GetAllEmail()
        {
            var model = await _getAllEmailServiceQuery.ExecuteAsync();
            return new OkObjectResult(model);
        }

        [HttpGet("GeEmailById")]
        public async Task<IActionResult> GeEmailById(int id)
        {
            var model = await _getEmailByIdService.ExecuteAsync(id);
            return new OkObjectResult(model);
        }

        [HttpGet("GetAllPaging")]
        public async Task<IActionResult> GetAllPaging(string keyword, int page, int pageSize)
        {
            var model = await _getAllPagingEmailService.ExecuteAsync(keyword, page, pageSize);
            return new OkObjectResult(model);
        }
        #endregion

        #region POST API
        [HttpPost("AddNewEmail")]
        public async Task<IActionResult> AddNewEmail([FromBody]EmailViewModel emailVm)
        {
            if (!ModelState.IsValid)
            {
                IEnumerable<ModelError> allErrors = ModelState.Values.SelectMany(v => v.Errors);
                return new BadRequestObjectResult(allErrors);
            }
            else
            {
                var execute = await _addNewEmailServiceCommand.ExecuteAsync(emailVm);
                return new OkObjectResult(execute);
            }
        }
        #endregion

        #region PUT API
        [HttpPut("UpdateEmail")]
        public async Task<IActionResult> UpdateEmail([FromBody]EmailViewModel emailVm)
        {
            if (!ModelState.IsValid)
            {
                IEnumerable<ModelError> allErrors = ModelState.Values.SelectMany(v => v.Errors);
                return new BadRequestObjectResult(allErrors);
            }
            else
            {
                var execute = await _updateNewEmailServiceCommand.ExecuteAsync(emailVm);
                return new OkObjectResult(execute);
            }
        }
        #endregion

        #region DELETE API
        [HttpDelete("DeleteEmail")]
        public async Task<IActionResult> DeleteEmail(int id)
        {
            if (!ModelState.IsValid)
            {
                return new BadRequestResult();
            }
            else
            {
                var execute = await _deleteEmailServiceCommand.ExecuteAsync(id);
                return new OkObjectResult(execute);
            }
        }
        #endregion
    }
}