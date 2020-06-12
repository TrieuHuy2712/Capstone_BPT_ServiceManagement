using BPT_Service.Model.Enums;
using System;
using System.Collections.Generic;

namespace BPT_Service.Application.PostService.ViewModel
{
    public class ListServiceViewModel
    {
        public Guid Id { get; set; }
        public string CategoryName { get; set; }
        public int CategoryId { get; set; }
        public Status Status { get; set; }
        public bool isProvider { get; set; }
        public string Author { get; set; }
        public string ServiceName { get; set; }
        public string AvtService { get; set; }
        public string TagList { get; set; }
        public string PriceOfService { get; set; }
        public double Rating { get; set; }
    }

    public class ListLocationPostViewModel
    {
        public string CategoryName { get; set; }
        public int CategoryId { get; set; }
        public List<ListServiceViewModel> ListService { get; set; }
    }
}