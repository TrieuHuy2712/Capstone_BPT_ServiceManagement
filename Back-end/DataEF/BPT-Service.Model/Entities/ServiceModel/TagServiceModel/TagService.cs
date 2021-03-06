using BPT_Service.Data.Infrastructure.SharedKernel;
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace BPT_Service.Model.Entities.ServiceModel
{
    [Table("TagService")]
    public class TagService : DomainEntity<int>
    {
        public Guid ServiceId { get; set; }

        public Guid TagId { get; set; }

        [ForeignKey("ServiceId")]
        public virtual Service Service { get; set; }

        [ForeignKey("TagId")]
        public virtual Tag Tag { get; set; }
    }
}