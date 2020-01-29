using System;

namespace BPT_Service.Application.RoleService.ViewModel
{
    public class AppRoleViewModel
    {
         public Guid? Id { set; get; }

        public string Name { set; get; }

        public string Description { set; get; }

        public PermissionSingleViewModel permissionSingleViewModel { set; get; }
    }
}