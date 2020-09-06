using BPT_Service.Data.Infrastructure.SharedKernel;
using BPT_Service.Model.Interfaces;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace BPT_Service.Model.Entities.ServiceModel.ProviderServiceModel
{
    [Table("ProviderFollowing")]
    public class ProviderFollowing : DomainEntity<int>, IDateTracking
    {
        public Guid UserId { get; set; }
        public Guid ProviderId { get; set; }
        public DateTime DateCreated { get; set; }
        public DateTime DateModified { get; set; }
        public bool IsReceiveEmail { get; set; }

        [ForeignKey("UserId")]
        public virtual AppUser AppUser { get; set; }

        [ForeignKey("ProviderId")]
        public virtual Provider Service { get; set; }
    }
}
