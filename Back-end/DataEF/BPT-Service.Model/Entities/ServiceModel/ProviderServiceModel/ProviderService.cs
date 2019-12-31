using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using BPT_Service.Data.Infrastructure.SharedKernel;

namespace BPT_Service.Model.Entities.ServiceModel
{
    [Table("ProviderService")]
    public class ProviderService: DomainEntity<int>
    {
        public Guid ServiceId { get; set; }

        public Guid ProviderId { get; set; }

        [ForeignKey("ServiceId")]
        public virtual Service Service { get; set;}

        [ForeignKey("ProviderId")]
        public virtual Provider Provider {get; set;}
    }
}