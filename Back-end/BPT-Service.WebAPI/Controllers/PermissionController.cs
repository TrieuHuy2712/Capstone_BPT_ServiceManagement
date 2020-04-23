using BPT_Service.Application.PermissionService.Query.GetPermissionRoleQuery;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace BPT_Service.WebAPI.Controllers
{
    [Authorize]
    [Route("PermissionManager")]
    public class PermissionController : ControllerBase
    {
        #region  Initialize
        private readonly IGetPermissionRoleQuery _permissionService;
        public PermissionController(IGetPermissionRoleQuery permissionService)
        {
            _permissionService = permissionService;
        }
        #endregion

        #region GET API
        [HttpGet("GetAllPermission/{functionId}")]
        public async Task<IActionResult> GetAllPermission(string functionId)
        {
            var model = await _permissionService.ExecuteAsync(functionId);
            return new OkObjectResult(model);
        }
        #endregion
    }
}