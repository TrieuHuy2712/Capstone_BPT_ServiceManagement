
using BPT_Service.Application.AuthenticateService.Command.ResetPasswordAsyncCommand;
using BPT_Service.Application.AuthenticateService.Query.AuthenticateofAuthenticationService;
using BPT_Service.Application.AuthenticateService.Query.GetAllAuthenticateService;
using BPT_Service.Application.AuthenticateService.Query.GetByIdAuthenticateService;
using BPT_Service.WebAPI.Models.AccountViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
namespace BPT_Service.WebAPI.Controllers
{
    [Authorize]
    [ApiController]
    [Route("[controller]")]
    public class AuthenticateController : ControllerBase
    {
        private readonly IResetPasswordAsyncCommand _resetPasswordCommand;
        private readonly IAuthenticateServiceQuery _authenticatQuery;
        private readonly IGetAllAuthenticateServiceQuery _getAllAuthenticateQuery;
        private readonly IGetByIdAuthenticateService _getByIdAuthenticate;

        public AuthenticateController(IResetPasswordAsyncCommand resetPasswordCommand,
        IAuthenticateServiceQuery authenticatQuery,
        IGetAllAuthenticateServiceQuery getAllAuthenticateQuery,
        IGetByIdAuthenticateService getByIdAuthenticate)
        {
            _resetPasswordCommand = resetPasswordCommand;
            _authenticatQuery = authenticatQuery;
            _getAllAuthenticateQuery = getAllAuthenticateQuery;
            _getByIdAuthenticate = getByIdAuthenticate;
        }

        #region  Post API
        [AllowAnonymous]
        [HttpPost("authenticate")]
        public async Task<IActionResult> Authenticate([FromBody]LoginViewModel model)
        {
            var user = await _authenticatQuery.ExecuteAsync(model.UserName, model.Password);

            if (user == null)
                return BadRequest(new { message = "Username or password is incorrect" });
            return Ok(user);

        }

        [HttpPost("changePassword")]
        public async Task<IActionResult> ChangePassword([FromBody]ChangePasswordViewModel model)
        {
            var user = await _resetPasswordCommand.ExecuteAsync(model.Username, model.OldPassword, model.NewPassword);
            return new OkObjectResult(user);
        }
        #endregion

        #region  GET API
        [HttpGet("getAll")]
        public async Task<IActionResult> GetAll()
        {
            var users = await _getAllAuthenticateQuery.ExecuteAsync();
            return Ok(users);
        }
        #endregion

    }
}