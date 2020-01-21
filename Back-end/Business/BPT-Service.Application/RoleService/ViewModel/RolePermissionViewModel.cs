using System.Collections.Generic;

namespace BPT_Service.Application.RoleService.ViewModel
{
    public class RolePermissionViewModel
    {
        public string FunctionId { get; set; }
        public IEnumerable<PermissionViewModel> Permissions { get; set; }
    }
}