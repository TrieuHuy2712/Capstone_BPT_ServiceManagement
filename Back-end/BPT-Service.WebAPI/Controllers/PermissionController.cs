using System.Threading.Tasks;
using BPT_Service.Application.PermissionService.Query.GetPermissionRoleQuery;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

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
        [HttpGet("GetAllPermission/{userName}/{functionId}")]
        public async Task<IActionResult> GetAllPermission(string userName, string functionId)
        {
            var model = _permissionService.ExecuteAsync(userName, functionId);
            return new OkObjectResult(model);
        }
        #endregion
    }
}