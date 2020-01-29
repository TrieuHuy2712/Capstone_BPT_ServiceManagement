using BPT_Service.Data.Infrastructure.SharedKernel;
using BPT_Service.Model.Entities.ServiceModel;
using BPT_Service.Model.Interfaces;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace BPT_Service.Model.Entities
{
    [Table("Tag")]
    public class Tag : DomainEntity<Guid>, IDateTracking
    {
        [Required]
        public string TagName { get; set; }

        public DateTime DateCreated { get; set; }

        public DateTime DateModified { get; set; }

        public ICollection<TagService> TagServices { get; set; }

        public Tag() { }
        public Tag(string tagName, string description)
        {
            TagName = tagName;
        }
    }
}
