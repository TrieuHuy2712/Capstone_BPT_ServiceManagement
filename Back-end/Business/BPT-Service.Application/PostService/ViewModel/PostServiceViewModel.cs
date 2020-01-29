using System;
using System.Collections.Generic;
using BPT_Service.Model.Enums;

namespace BPT_Service.Application.PostService.ViewModel
{
    public class PostServiceViewModel
    {
        public string ServiceName { get; set; }

        public int CategoryId { set; get; }

        public string Description { get; set; }

        public decimal PriceOfService { get; set; }

        public DateTime DateCreated { get; set; }

        public DateTime DateModified { get; set; }

        public Status Status { get; set; }
        public List<PostServiceImageViewModel> listImages { get; set; }
        public List<ServiceofProviderViewModel> serviceofProvider { get; set; }
        public List<TagofServiceViewModel> tagofServices { get; set; }

    }
    public class PostServiceImageViewModel
    {
        public string Path { get; set; }
        public DateTime DateCreated { get; set; }
        public DateTime DateModified { get; set; }
        public Status Status { get; set; }

        public Guid ServiceId { get; set; }
    }

    public class ServiceofProviderViewModel
    {
        public Guid ServiceId { get; set; }

        public Guid ProviderId { get; set; }
    }
    public class TagofServiceViewModel
    {
        public bool isAdd;
        public Guid ServiceId { get; set; }

        public Guid TagId { get; set; }
    }
}