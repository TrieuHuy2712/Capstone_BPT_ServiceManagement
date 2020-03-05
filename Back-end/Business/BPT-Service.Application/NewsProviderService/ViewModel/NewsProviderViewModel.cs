using System;
using BPT_Service.Model.Enums;

namespace BPT_Service.Application.NewsProviderService.ViewModel
{
    public class NewsProviderViewModel
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Author { get; set; }
        public string Content { get; set; }
        public string UserId { get; set; }
        public DateTime DateCreated { get; set; }
        public DateTime DateModified { get; set; }
        public Status Status { get; set; }
        public string ProviderId { get; set; }
        public string ProviderName { get; set; }
        public string Reason { get; set; }
    }
}