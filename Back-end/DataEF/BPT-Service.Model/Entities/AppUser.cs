using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using BPT_Service.Model.Entities.ServiceModel;
using BPT_Service.Model.Entities.ServiceModel.UserServiceModel;
using BPT_Service.Model.Enums;
using BPT_Service.Model.Interfaces;
using Microsoft.AspNetCore.Identity;

namespace BPT_Service.Model.Entities
{
    [Table("AppUsers")]
    public class AppUser:IdentityUser<Guid>, IDateTracking, ISwitchable
    {
        public AppUser() {  }
        public AppUser(Guid id, string fullName, string userName, 
            string email, string phoneNumber, string avatar, Status status)
        {
            Id = id;
            FullName = fullName;
            UserName = userName;
            Email = email;
            PhoneNumber = phoneNumber;
            Avatar = avatar;
            Status = status;
        }
        public string FullName { get; set; }

        public string Token { get; set; }

        public string Avatar { get; set; }

        public DateTime DateCreated { get; set; }
        public DateTime DateModified { get; set; }
        public Status Status { get; set; }
        public ICollection<Provider> Providers{ get; set; }
        public ICollection<UserService> UserServices { get; set; }
        public ICollection<ServiceFollowing> ServiceFollowings { get; set; }
        public ICollection<ServiceRating> ServiceRatings { get; set; }
        public ICollection<ServiceComment> ServiceComments { get; set; }
    }
}