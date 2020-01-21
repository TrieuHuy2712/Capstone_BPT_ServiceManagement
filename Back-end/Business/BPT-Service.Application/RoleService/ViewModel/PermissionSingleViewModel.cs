using System;

namespace BPT_Service.Application.RoleService.ViewModel
{
    public class PermissionSingleViewModel
    {
        public int Id { get; set; }

        public Guid RoleId { get; set; }

        public string FunctionId { get; set; }

        public bool CanCreate { set; get; }

        public bool CanRead { set; get; }

        public bool CanUpdate { set; get; }

        public bool CanDelete { set; get; }
    }
}