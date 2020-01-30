using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using BPT_Service.Data.Infrastructure.SharedKernel;

namespace BPT_Service.Model.Entities.ServiceModel.UserServiceModel
{
    [Table("UserService")]
    public class UserService: DomainEntity<int>
    {
        public Guid ServiceId { get; set; }
        public Guid UserId { get; set; }

        [ForeignKey("ServiceId")]
        public virtual Service Service { get; set; }

        [ForeignKey("UserId")]
        public virtual AppUser AppUser { get; set; }
    }
}