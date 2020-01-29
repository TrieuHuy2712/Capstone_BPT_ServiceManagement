using System;
using System.Collections.Generic;
using BPT_Service.Model.Enums;

namespace BPT_Service.Application.UserService.ViewModel
{
    public class AppUserViewModelinUserService
    {
        public AppUserViewModelinUserService()
        {
            Roles = new List<string>();
        }
        public Guid? Id { set; get; }
        public string FullName { set; get; }
        public string Email { set; get; }
        public string Password { set; get; }
        public string UserName { set; get; }
        public string Avatar { get; set; }
        public Status Status { get; set; }
        public string Token { get; set; }
        public string Expiration { get; set; }
        public string PhoneNumber { get; set; }

        public DateTime DateCreated { get; set; }

        public List<string> Roles { get; set; }

        public List<string> NewRoles { get; set; }
    }
    public class UserRoleViewModel
    {
        public Guid MyProperty { get; set; }
    }
}