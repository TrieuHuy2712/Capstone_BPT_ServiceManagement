using BPT_Service.Data.Infrastructure.SharedKernel;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace BPT_Service.Model.Entities
{
    [Table("Tag")]
    public class Tag: DomainEntity<int>
    {
        [Required]
        public string TagName { get; set; }

        [StringLength(200)]
        public string Description { get; set; }

        public Tag() {  }
        public Tag(string tagName, string description) {
            TagName = tagName;
            Description = description;
        }
    }
}
