using BPT_Service.Data.Infrastructure.SharedKernel;
using BPT_Service.Model.Enums;

namespace BPT_Service.Model.Entities
{
    public class Recommendation : DomainEntity<int>
    {
        public string IdType { get; set; }
        public TypeRecommendation Type { get; set; }
        public int Order { get; set; }
    }
}