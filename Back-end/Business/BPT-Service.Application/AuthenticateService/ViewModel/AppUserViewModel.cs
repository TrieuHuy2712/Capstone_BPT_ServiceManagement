using System;
using System.Collections.Generic;
using BPT_Service.Model.Enums;

namespace BPT_Service.Application.AuthenticateService.ViewModel
{
    public class AppUserViewModel
    {
        public AppUserViewModel()
        {
            Roles = new List<string>();
        }
        public Guid? Id { set; get; }
        public string FullName { set; get; }
        public string BirthDay { set; get; }
        public string Email { set; get; }
        public string Password { set; get; }
        public string UserName { set; get; }
        public AppUserViewModel(string address, string avatar, Status status, string token, string expiration, string gender, DateTime dateCreated)
        {
            this.Address = address;
            this.Avatar = avatar;
            this.Status = status;
            this.Token = token;
            this.Expiration = expiration;
            this.Gender = gender;
            this.DateCreated = dateCreated;

        }
        public string Address { get; set; }
        public string PhoneNumber { set; get; }
        public string Avatar { get; set; }
        public Status Status { get; set; }
        public string Token { get; set; }
        public string Expiration { get; set; }
        public string Gender { get; set; }

        public DateTime DateCreated { get; set; }

        public List<string> Roles { get; set; }

        public List<string> NewRoles { get; set; }
    }
    public class UserRoleViewModel
    {
        public Guid MyProperty { get; set; }
    }
}