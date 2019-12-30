using System.Threading.Tasks;
using BPT_Service.Application.Interfaces;
using BPT_Service.WebAPI.Models.AccountViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BPT_Service.WebAPI.Controllers
{
    [Authorize]
    [ApiController]
    [Route("[controller]")]
    public class AuthenticateController : ControllerBase
    {
        private  readonly IAuthenticateService _userService;

        public AuthenticateController(IAuthenticateService userService)
        {
            _userService = userService;
        }

        #region  Post API
        [AllowAnonymous]
        [HttpPost("authenticate")]
        public IActionResult Authenticate([FromBody]LoginViewModel model)
        {
            var user = _userService.Authenticate(model.UserName, model.Password);

            if (user == null)
                return BadRequest(new { message = "Username or password is incorrect" });
            return Ok(user.Result);

        }

        [HttpPost("changePassword")]
        public async Task<IActionResult> ChangePassword(string username, string oldPassword, string newPassword)
        {
            var user = await _userService.ResetPasswordAsync(username, oldPassword, newPassword);
            return new OkObjectResult(user);
        }
        #endregion

        #region  GET API
        [HttpGet("getAll")]
        public IActionResult GetAll()
        {
            var users = _userService.GetAll();
            return Ok(users);
        }
        #endregion

    }
}