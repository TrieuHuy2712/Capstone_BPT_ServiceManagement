using System;
using BPT_Service.Application.RoleService.ViewModel;

namespace BPT_Service.Application.FunctionService.ViewModel
{
    public class AppRoleViewModelinFunctionService
    {
        public Guid? Id { set; get; }

        public string Name { set; get; }

        public string NameVietNamese { set; get; }

        public string Description { set; get; }

        public PermissionSingleViewModel permissionSingleViewModel { set; get; }
    }
}
