using BPT_Service.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BPT_Service.WebAPI.Controllers
{
    [Authorize]
    [Route("PermissionManager")]
    public class PermissionController : ControllerBase
    {
        #region  Initialize
        private readonly IPermissionService _permissionService;
        public PermissionController(IPermissionService permissionService)
        {
            _permissionService = permissionService;
        }
        #endregion

        #region GET API
        [HttpGet("GetAllPermission/{userName}/{functionId}")]
        public IActionResult GetAllPermission(string userName, string functionId)
        {
            var model = _permissionService.GetPermissionRole(userName, functionId);
            return new OkObjectResult(model);
        }
        #endregion
    }
}