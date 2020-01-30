using System;
using BPT_Service.Model.Enums;

namespace BPT_Service.Application.ProviderService.ViewModel
{
    public class ProviderServiceViewModel
    {
        public Guid Id { get; set; }
        public string ProviderName { get; set; }
        public Guid UserId { get; set; }
        public string UserName { get; set; }
        public string TaxCode { get; set; }
        public int CityId { get; set; }
        public string PhoneNumber { get; set; }
        public string Address { get; set; }
        public string Description { get; set; }
        public DateTime DateCreated { get; set; }
        public DateTime DateModified { get; set; }
        public Status Status { get; set; }
        public string Reason { get; set; }
        public LocationCityViewModel LocationCity { get; set; }
    }
}