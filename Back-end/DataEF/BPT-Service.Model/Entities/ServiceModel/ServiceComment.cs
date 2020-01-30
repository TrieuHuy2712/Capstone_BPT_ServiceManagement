using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using BPT_Service.Data.Infrastructure.SharedKernel;
using BPT_Service.Model.Interfaces;

namespace BPT_Service.Model.Entities.ServiceModel
{
    [Table("ServiceComment")]
    public class ServiceComment : DomainEntity<Guid>, IDateTracking
    {

        public ServiceComment(Guid userId, Guid serviceId, Guid parentId, string contentOfRating, DateTime dateCreated, DateTime dateModified)
        {
            this.UserId = userId;
            this.ServiceId = serviceId;
            this.ParentId = parentId;
            this.ContentOfRating = contentOfRating;
            this.DateCreated = dateCreated;
            this.DateModified = dateModified;

        }
        public Guid UserId { get; set; }
        public Guid ServiceId { get; set; }

        public Guid ParentId { get; set; }

        public string ContentOfRating { get; set; }

        public DateTime DateCreated { get; set; }
        public DateTime DateModified { get; set; }
        [ForeignKey("UserId")]
        public virtual AppUser AppUser { get; set; }

        [ForeignKey("ServiceId")]
        public virtual Service Service { get; set; }
    }
}