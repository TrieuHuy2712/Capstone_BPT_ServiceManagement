using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System;
using BPT_Service.Application.RoleService.Command.AddRoleAsync;
using BPT_Service.Application.RoleService.Command.DeleteRoleAsync;
using BPT_Service.Application.RoleService.Command.SavePermissionRole;
using BPT_Service.Application.RoleService.Command.UpdateRoleAsync;
using BPT_Service.Application.RoleService.Query.GetAllPermission;
using BPT_Service.Application.RoleService.Query.GetAllAsync;
using BPT_Service.Application.RoleService.Query.GetAllPagingAsync;
using BPT_Service.Application.RoleService.Query.GetListFunctionWithRole;
using BPT_Service.Application.RoleService.Query.GetByIdAsync;
using BPT_Service.Application.RoleService.ViewModel;

namespace BPT_Service.WebAPI.Controllers
{
    [Authorize]
    [ApiController]
    [Route("AdminRole")]
    public class RoleController : ControllerBase
    {
        #region Constructor
        private readonly IAddRoleAsyncCommand _addRoleService;
        private readonly IDeleteRoleAsyncCommand _deleteRoleService;
        private readonly ISavePermissionCommand _savePerRoleService;
        private readonly IUpdateRoleAsyncCommand _updateRoleService;
        private readonly IGetAllPermissionQuery _getAllPermissionService;
        private readonly IGetAllRoleAsyncQuery _getAllRoleService;
        private readonly IGetAllRolePagingAsyncQuery _getAllPagingRoleService;
        private readonly IGetListFunctionWithRoleQuery _getListFunctionwithRoleService;
        private readonly IGetRoleByIdAsyncQuery _getByIdRoleService;

        public RoleController(IAddRoleAsyncCommand addRoleService,
        IDeleteRoleAsyncCommand deleteRoleService,
        ISavePermissionCommand savePerRoleService,
        IUpdateRoleAsyncCommand updateRoleService,
        IGetAllPermissionQuery getAllPermissionService,
        IGetAllRoleAsyncQuery getAllRoleService,
        IGetAllRolePagingAsyncQuery getAllPagingRoleService,
        IGetListFunctionWithRoleQuery getListFunctionwithRoleService,
        IGetRoleByIdAsyncQuery getByIdRoleService)
        {
            _addRoleService = addRoleService;
            _deleteRoleService = deleteRoleService;
            _savePerRoleService = savePerRoleService;
            _updateRoleService = updateRoleService;
            _getAllPermissionService = getAllPermissionService;
            _getAllRoleService = getAllRoleService;
            _getAllPagingRoleService = getAllPagingRoleService;
            _getListFunctionwithRoleService = getListFunctionwithRoleService;
            _getByIdRoleService = getByIdRoleService;
        }
        #endregion

        #region GET API
        [HttpGet("GetAll")]
        public async Task<IActionResult> GetAll()
        {
            var model = await _getAllRoleService.ExecuteAsync();

            return new OkObjectResult(model);
        }

        [HttpGet("GetById/{id}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var model = await _getByIdRoleService.ExecuteAsync(id);

            return new OkObjectResult(model);
        }

        [HttpGet("GetAllPaging")]
        public async Task<IActionResult> GetAllPaging(string keyword, int page, int pageSize)
        {
            var model = _getAllPagingRoleService.ExecuteAsync(keyword, page, pageSize);
            return new OkObjectResult(model);
        }

        [HttpGet("getAllPermission/{functionId}")]
        public async Task<IActionResult> GetAllPermission(string functionId)
        {
            var function = await _getAllPermissionService.ExecuteAsync(functionId);
            return new OkObjectResult(function);
        }
        #endregion

        #region POST API
        [HttpPost("SaveEntity")]
        public async Task<IActionResult> SaveEntity([FromBody]AppRoleViewModel roleVm)
        {
            if (!ModelState.IsValid)
            {
                IEnumerable<ModelError> allErrors = ModelState.Values.SelectMany(v => v.Errors);
                return new BadRequestObjectResult(allErrors);
            }
            if (!roleVm.Id.HasValue)
            {
                var notificationId = Guid.NewGuid().ToString();
                await _addRoleService.ExecuteAync(roleVm);
            }
            else
            {
                await _updateRoleService.ExecuteAsync(roleVm);
            }
            return new OkObjectResult(roleVm);
        }

        [HttpPost("ListAllFunction/{roleId}")]
        public IActionResult ListAllFunction([FromBody]Guid roleId)
        {
            var functions = _getListFunctionwithRoleService.ExecuteAsync(roleId);
            return new OkObjectResult(functions);
        }

        [HttpPost("SavePermission")]
        public IActionResult SavePermission([FromBody]RolePermissionViewModel rolePermissionViewModel)
        {
            _savePerRoleService.ExecuteAsync(rolePermissionViewModel);
            return new OkObjectResult(rolePermissionViewModel);
        }
        #endregion

        #region DELETE API
        [HttpDelete("Delete")]
        public async Task<IActionResult> Delete(Guid id)
        {
            if (!ModelState.IsValid)
            {
                return new BadRequestObjectResult(ModelState);
            }
            await _deleteRoleService.ExecuteAsync(id);
            return new OkObjectResult(id);
        }
        #endregion
    }
}