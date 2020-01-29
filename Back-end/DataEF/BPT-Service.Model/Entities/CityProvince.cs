using System.Collections.Generic;
using BPT_Service.Data.Infrastructure.SharedKernel;
using BPT_Service.Model.Entities.ServiceModel;

namespace BPT_Service.Model.Entities
{
    public class CityProvince : DomainEntity<int>
    {
        public string City { get; set; }
        public string Province { get; set; }
        public ICollection<Provider> Providers { get; set; }

    }
}