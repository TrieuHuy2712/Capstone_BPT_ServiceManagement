using BPT_Service.Model.Enums;
using System;
using System.Collections.Generic;

namespace BPT_Service.Application.PostService.ViewModel
{
    public class PostServiceViewModel
    {
        public string Id { get; set; }
        public string ServiceName { get; set; }
        public string Author { get; set; }
        public string ProviderId { get; set; }
        public string UserId { get; set; }
        public int CategoryId { set; get; }
        public string CategoryName { get; set; }
        public string Description { get; set; }
        public string AvtService { get; set; }
        public string PriceOfService { get; set; }
        public DateTime DateCreated { get; set; }
        public DateTime DateModified { get; set; }
        public string Email { get; set; }
        public string Reason { get; set; }
        public string TagList { get; set; }
        public Status Status { get; set; }
        public double Rating { get; set; }
        public bool IsProvider { get; set; }
        public List<PostServiceImageViewModel> listImages { get; set; }
        public ServiceofProviderViewModel serviceofProvider { get; set; }
        public List<TagofServiceViewModel> tagofServices { get; set; }
        public ServiceofUserViewModel userofServices { get; set; }
    }

    public class PostServiceImageViewModel
    {
        public int ImageId { get; set; }
        public string Path { get; set; }
        public DateTime DateCreated { get; set; }
        public DateTime DateModified { get; set; }
        public Status Status { get; set; }
        public bool IsAvatar { get; set; }

        public Guid ServiceId { get; set; }
    }

    public class ServiceofProviderViewModel
    {
        public string ServiceId { get; set; }

        public string ProviderId { get; set; }
    }

    public class ServiceofUserViewModel
    {
        public string ServiceId { get; set; }

        public string UserId { get; set; }
    }

    public class TagofServiceViewModel
    {
        public bool isAdd { get; set; }
        public bool isDelete { get; set; }
        public string TagName { get; set; }
        public string ServiceId { get; set; }

        public string TagId { get; set; }
    }
}