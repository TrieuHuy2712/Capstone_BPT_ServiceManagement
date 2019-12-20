using System;
using System.Collections.Generic;
using System.Text;

namespace BPT_Service.Application.ViewModels.System
{
    public class RolePermissionViewModel
    {
        public string FunctionId { get; set; }
        public IEnumerable<PermissionViewModel> Permissions { get; set; }
    }
}
