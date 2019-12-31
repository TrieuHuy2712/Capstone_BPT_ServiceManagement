using System;
using System.ComponentModel.DataAnnotations.Schema;
using BPT_Service.Model.Enums;
using BPT_Service.Model.Interfaces;
using Microsoft.AspNetCore.Identity;

namespace BPT_Service.Model.Entities.ServiceModel
{
    [Table("ServiceImage")]
    public class ServiceImage : IdentityUser<Guid>, IDateTracking, ISwitchable
    {
        public string Path{get; set; }
        public DateTime DateCreated { get; set; }
        public DateTime DateModified { get; set; }
        public Status Status { get; set; }

        public Guid ServiceId {get; set;}
        
        [ForeignKey("ServiceId")]
        public virtual Service Service { get; set; }
    }
}