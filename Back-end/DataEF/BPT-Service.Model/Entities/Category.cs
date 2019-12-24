using BPT_Service.Data.Infrastructure.SharedKernel;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace BPT_Service.Model.Entities
{
    [Table("Category")]
    public class Category: DomainEntity<int>
    {
        [Required]
        public string CategoryName { get; set; }

        [Required]
        public string NameVietnamese { get; set; }

        [StringLength(200)]
        public string Description { get; set; }

        public Category() {  }
        public Category(string categoryName, string vietnameseName, string description) {
            CategoryName = categoryName;
            NameVietnamese = vietnameseName;
            Description = description;
        }
    }
}
