using System;

namespace BPT_Service.Application.RoleService.ViewModel
{
    public class PermissionViewModel
    {
        public int Id { get; set; }

        public Guid RoleId { get; set; }

        public string FunctionId { get; set; }

        public bool CanCreate { set; get; }

        public bool CanRead { set; get; }

        public bool CanUpdate { set; get; }

        public bool CanDelete { set; get; }

        public string RoleName { get; set; }
        public AppRoleViewModel AppRole { get; set; }

        public FunctionViewModel Function { get; set; }
    }
}