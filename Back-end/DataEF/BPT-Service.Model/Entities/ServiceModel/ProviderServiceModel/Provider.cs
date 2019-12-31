using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using BPT_Service.Data.Infrastructure.SharedKernel;
using BPT_Service.Model.Entities.ServiceModel.ProviderServiceModel;
using BPT_Service.Model.Enums;
using BPT_Service.Model.Interfaces;

namespace BPT_Service.Model.Entities.ServiceModel
{
    [Table("Provider")]
    public class Provider : DomainEntity<Guid>, IDateTracking, ISwitchable
    {
        [Required]
        public string ProviderName { get; set; }

        public Guid UserId { get; set; }

        [Required]
        public string TaxCode { get; set; }

        [Required]
        public string PhoneNumber { get; set; }

        [Required]
        public string Address { get; set; }

        public string Description { get; set; }

        [ForeignKey("UserId")]
        public virtual AppUser AppUser { get; set; }
        public Status Status { get; set; }
        public DateTime DateCreated { get; set; }
        public DateTime DateModified { get; set; }

        public ICollection<ProviderService> ProviderServices { get; set; }
        public ICollection<ProviderNew> ProviderNews {get; set;}
    }
}