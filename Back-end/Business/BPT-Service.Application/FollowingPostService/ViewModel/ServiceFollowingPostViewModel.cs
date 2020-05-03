using System;
using System.Collections.Generic;
using System.Text;

namespace BPT_Service.Application.FollowingPostService.ViewModel
{
    public class ServiceFollowingPostViewModel
    {
        public int Id { get; set; }
        public string UserId { get; set; }
        public string ServiceId { get; set; }
        public DateTime DateCreated { get; set; }
        public DateTime DateModified { get; set; }
        public string UserName { get; set; }
        public string AvtService { get; set; }
    }
}
