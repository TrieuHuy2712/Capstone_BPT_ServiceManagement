using System;
using BPT_Service.Model.Enums;

namespace BPT_Service.Application.PostService.ViewModel
{
    public class ListServiceViewModel
    {
        public Guid Id { get; set; }
        public string CategoryName {get; set;}
        public Status Status { get; set; }
        public bool isProvider { get; set; }
        public string Author {get; set;}
        public string ServiceName{get;set;}
        public string AvtService { get; set; }
        public string TagList { get; set; }
        public string PriceOfService { get; set; }
        public double Rating { get; set; }
    }
}