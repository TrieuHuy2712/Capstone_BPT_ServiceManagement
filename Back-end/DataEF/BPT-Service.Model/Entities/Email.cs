using BPT_Service.Data.Infrastructure.SharedKernel;
using System;
using System.Collections.Generic;
using System.Text;

namespace BPT_Service.Model.Entities
{
    public class Email : DomainEntity<int>
    {
        public string Name { get; set; }
        public string To { get; set; }
        public string Subject { get; set; }
        public string Message { get; set; }
    }
}
