using BPT_Service.Model.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace BPT_Service.WebAPI.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly SignInManager<AppUser> _signInManager;

        public AccountController(SignInManager<AppUser> signInManager)
        {
            _signInManager = signInManager;
        }

    //     [HttpPost]
    //     public async Task<IActionResult> Logout()
    //     {
    //         await _signInManager.SignOutAsync();
    //         return Redirect("/Admin/Login/Index");
    //     }
    }
}