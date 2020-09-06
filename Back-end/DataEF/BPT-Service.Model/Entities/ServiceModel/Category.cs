using BPT_Service.Data.Infrastructure.SharedKernel;
using BPT_Service.Model.Entities.ServiceModel;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BPT_Service.Model.Entities
{
    [Table("Category")]
    public class Category : DomainEntity<int>
    {
        [Required]
        public string CategoryName { get; set; }

        [StringLength(200)]
        public string Description { get; set; }

        public string ImgPath { get; set; }

        public ICollection<Service> Services { get; set; }

        public Category()
        {
        }
    }
}