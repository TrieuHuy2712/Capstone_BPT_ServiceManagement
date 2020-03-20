using BPT_Service.Data.Infrastructure.SharedKernel;
using BPT_Service.Model.Interfaces;
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace BPT_Service.Model.Entities.ServiceModel
{
    [Table("ServiceRating")]
    public class ServiceRating : DomainEntity<int>, IDateTracking
    {
        public Guid UserId { get; set; }
        public Guid ServiceId { get; set; }

        public int NumberOfRating { get; set; }
        public DateTime DateCreated { get; set; }
        public DateTime DateModified { get; set; }

        [ForeignKey("UserId")]
        public virtual AppUser AppUser { get; set; }

        [ForeignKey("ServiceId")]
        public virtual Service Service { get; set; }
    }
}