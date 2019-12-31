using System.Collections.Generic;
using System.Threading.Tasks;
using BPT_Service.Application.Interfaces;
using BPT_Service.Application.ViewModels.System;
using BPT_Service.Common.Dtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BPT_Service.WebAPI.Controllers
{
    [Authorize]
    [Route("UserManagement")]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;
        public UserController(IUserService userService)
        {
            _userService = userService;
        }

        #region GET API
        [HttpGet("GetAllUser")]
        public async Task<IActionResult> GetAllUser()
        {
            var model = await _userService.GetAllAsync();
            return new ObjectResult(model);
        }

        [HttpGet("GetAllPaging")]
        public async Task<IActionResult> GetAllPagingUser(string keyword, int page, int pageSize)
        {
            var model = _userService.GetAllPagingAsync(keyword, page, pageSize);
            return new ObjectResult(model);
        }

        [HttpGet("GetById/{id}")]
        public async Task<IActionResult> GetById(string id)
        {
            var model = await _userService.GetById(id);
            return new ObjectResult(model);
        }
        #endregion

        #region PUT API
        [HttpPut("UpdateUser")]
        public async Task<IActionResult> UpdateUser([FromBody]AppUserViewModel userVm)
        {
            var model = _userService.UpdateAsync(userVm);
            return new ObjectResult(model);
        }
        #endregion

        #region POST API
        [HttpPost("CreateNewuser")]
        public async Task<IActionResult> CreateNewuser([FromBody]AppUserViewModel userVm, string password)
        {
            var model = _userService.CreateCustomerAsync(userVm, password);
            return new ObjectResult(model);
        }

        [AllowAnonymous]
        [HttpPost("LoginExternal")]
        public async Task<IActionResult> LoginExternal([FromBody]AppUserViewModel userViewModel)
        {
            var model = _userService.AddExternalAsync(userViewModel);
            return new ObjectResult(model.Result);
        }

        [HttpPost("AddNewUser")]
        public async Task<IActionResult> AddNewUser([FromBody]AppUserViewModel userVm)
        {
            var model = await _userService.AddAsync(userVm);
            return new ObjectResult(model);
        }
        #endregion

        #region DELETE API
        [HttpDelete("DeleteUser")]
        public async Task<IActionResult> DeleteUser(string id)
        {
            var model = _userService.DeleteAsync(id);
            return new ObjectResult(model);
        }
        #endregion
    }
}