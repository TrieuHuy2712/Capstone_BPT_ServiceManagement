using System.Threading.Tasks;
using BPT_Service.Application.UserService.Command.AddCustomerAsync;
using BPT_Service.Application.UserService.Command.AddExternalAsync;
using BPT_Service.Application.UserService.Command.AddUserAsync;
using BPT_Service.Application.UserService.Command.DeleteUserAsync;
using BPT_Service.Application.UserService.Command.UpdateUserAsync;
using BPT_Service.Application.UserService.Query.GetAllAsync;
using BPT_Service.Application.UserService.Query.GetAllPagingAsync;
using BPT_Service.Application.UserService.Query.GetByIdAsync;
using BPT_Service.Application.UserService.ViewModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BPT_Service.WebAPI.Controllers
{
    [Authorize]
    [Route("UserManagement")]
    public class UserController : ControllerBase
    {
        private readonly IAddCustomerAsyncCommand _addCustomerService;
        private readonly IAddExternalAsyncCommand _addExternalService;
        private readonly IAddUserAsyncCommand _addUserService;
        private readonly IDeleteUserAsyncCommand _deleteUserService;
        private readonly IUpdateUserAsyncCommand _updateUserService;
        private readonly IGetAllPagingUserAsyncQuery _getPagingUserService;
        private readonly IGetAllUserAsyncQuery _getAllUserService;
        private readonly IGetByIdUserAsyncQuery _getByIdUserService;
        public UserController(IAddCustomerAsyncCommand addCustomerService,
        IAddExternalAsyncCommand addExternalService,
        IAddUserAsyncCommand addUserService,
        IDeleteUserAsyncCommand deleteUserService,
        IUpdateUserAsyncCommand updateUserService,
        IGetAllPagingUserAsyncQuery getPagingUserService,
        IGetAllUserAsyncQuery getAllUserService,
        IGetByIdUserAsyncQuery getByIdUserService)
        {
            _addCustomerService = addCustomerService;
            _addExternalService = addExternalService;
            _addUserService = addUserService;
            _deleteUserService = deleteUserService;
            _updateUserService = updateUserService;
            _getPagingUserService = getPagingUserService;
            _getAllUserService = getAllUserService;
            _getByIdUserService = getByIdUserService;
        }

        #region GET API
        [HttpGet("GetAllUser")]
        public async Task<IActionResult> GetAllUser()
        {
            var model = await _getAllUserService.ExecuteAsync();
            return new ObjectResult(model);
        }

        [HttpGet("GetAllPaging")]
        public async Task<IActionResult> GetAllPagingUser(string keyword, int page, int pageSize)
        {
            var model = await _getPagingUserService.ExecuteAsync(keyword, page, pageSize);
            return new ObjectResult(model);
        }

        [HttpGet("GetById")]
        public async Task<IActionResult> GetById(string id)
        {
            var model = await _getByIdUserService.ExcecuteAsync(id);
            return new ObjectResult(model);
        }
        #endregion

        #region PUT API
        [HttpPut("UpdateUser")]
        public async Task<IActionResult> UpdateUser([FromBody]AppUserViewModelinUserService userVm)
        {
            var model = await _updateUserService.ExecuteAsync(userVm);
            return new ObjectResult(model);
        }
        #endregion

        #region POST API
        [HttpPost("CreateNewuser")]
        public async Task<IActionResult> CreateNewuser([FromBody]AppUserViewModelinUserService userVm)
        {
            var model = await _addCustomerService.ExecuteAsync(userVm);
            return new ObjectResult(model);
        }

        [AllowAnonymous]
        [HttpPost("LoginExternal")]
        public async Task<IActionResult> LoginExternal([FromBody]AppUserViewModelinUserService userViewModel)
        {
            var model = await _addExternalService.ExecuteAsync(userViewModel);
            return new ObjectResult(model);
        }

        [HttpPost("AddNewUser")]
        public async Task<IActionResult> AddNewUser([FromBody]AppUserViewModelinUserService userVm)
        {
            var model = await _addUserService.ExecuteAsync(userVm);
            return new ObjectResult(model);
        }
        #endregion

        #region DELETE API
        [HttpDelete("DeleteUser")]
        public async Task<IActionResult> DeleteUser(string id)
        {
            var model = await _deleteUserService.ExecuteAsync(id);
            return new ObjectResult(model);
        }
        #endregion
    }
}