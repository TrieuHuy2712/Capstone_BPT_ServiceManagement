using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using BPT_Service.Data.Infrastructure.SharedKernel;
using BPT_Service.Model.Entities.ServiceModel.ProviderServiceModel;
using BPT_Service.Model.Entities.ServiceModel.UserServiceModel;
using BPT_Service.Model.Enums;
using BPT_Service.Model.Interfaces;

namespace BPT_Service.Model.Entities.ServiceModel
{
    [Table("Service")]
    public class Service : DomainEntity<Guid>, IDateTracking, ISwitchable
    {
        [Required]
        public string ServiceName { get; set; }

        [Required]
        public int CategoryId { set; get; }

        [Required]
        public string Description { get; set; }

        [Required]
        public decimal PriceOfService { get; set; }

        [ForeignKey("CategoryId")]
        public virtual Category ServiceCategory { get; set; }

        public DateTime DateCreated { get; set; }

        public DateTime DateModified { get; set; }

        public Status Status { get; set; }

        public ICollection<ProviderService> ProviderServices { get; set; }

        public ICollection<UserService> UserServices { get; set; }

        public ICollection<TagService> TagServices { get; set; }

        public ICollection<ServiceImage> ServiceImages { get; set; }

        public ICollection<ServiceFollowing> ServiceFollowings { get; set; }

        public ICollection<ServiceRating> ServiceRatings { get; set; }

        public ICollection<ServiceComment> ServiceComments { get; set; }

        public Service() { }
        public Service(string serviceName, string description, decimal priceOfService)
        {
            ServiceName = serviceName;
            Description = description;
            PriceOfService = priceOfService;
        }
    }
}