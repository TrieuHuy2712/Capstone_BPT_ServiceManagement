using System;
using System.ComponentModel.DataAnnotations.Schema;
using BPT_Service.Data.Infrastructure.SharedKernel;
using BPT_Service.Model.Enums;
using BPT_Service.Model.Interfaces;

namespace BPT_Service.Model.Entities.ServiceModel.ProviderServiceModel
{
    public class ProviderNew : DomainEntity<int>, IDateTracking, ISwitchable
    {
        public string Title { get; set; }
        public string Author { get; set; }
        public string Content {get; set; }
        public DateTime DateCreated { get; set; }
        public DateTime DateModified { get; set; }
        public Status Status { get; set; }

        public Guid ProviderId {get; set;}

        [ForeignKey("ProviderId")]
        public virtual Provider Provider {get; set;}
    }
}