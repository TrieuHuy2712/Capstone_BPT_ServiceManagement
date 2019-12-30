using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BPT_Service.Application.Interfaces;
using BPT_Service.Application.ViewModels.System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Authorization;
using Newtonsoft.Json.Linq;

namespace BPT_Service.WebAPI.Controllers
{
    [Authorize]
    [ApiController]
    [Route("AdminRole")]
    public class RoleController : ControllerBase
    {
        #region Constructor
        private readonly IRoleService _roleService;

        public RoleController(IRoleService roleService)
        {
            _roleService = roleService;
        }
        #endregion

        #region GET API
        [HttpGet("GetAll")]
        public async Task<IActionResult> GetAll()
        {
            var model = await _roleService.GetAllAsync();

            return new OkObjectResult(model);
        }

        [HttpGet("GetById/{id}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var model = await _roleService.GetById(id);

            return new OkObjectResult(model);
        }

        [HttpGet("GetAllPaging")]
        public IActionResult GetAllPaging(string keyword, int page, int pageSize)
        {
            var model = _roleService.GetAllPagingAsync(keyword, page, pageSize);
            return new OkObjectResult(model);
        }

        [HttpGet("getAllPermission/{functionId}")]
        public IActionResult GetAllPermission(string functionId)
        {
            var function = _roleService.GetAllPermission(functionId);
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
                await _roleService.AddAsync(roleVm);
            }
            else
            {
                await _roleService.UpdateAsync(roleVm);
            }
            return new OkObjectResult(roleVm);
        }

        [HttpPost("ListAllFunction/{roleId}")]
        public IActionResult ListAllFunction([FromBody]Guid roleId)
        {
            var functions = _roleService.GetListFunctionWithRole(roleId);
            return new OkObjectResult(functions);
        }

        [HttpPost("SavePermission")]
        public IActionResult SavePermission([FromBody]RolePermissionViewModel rolePermissionViewModel)
        {
            _roleService.SavePermission(rolePermissionViewModel);
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
            await _roleService.DeleteAsync(id);
            return new OkObjectResult(id);
        }
        #endregion
    }
}