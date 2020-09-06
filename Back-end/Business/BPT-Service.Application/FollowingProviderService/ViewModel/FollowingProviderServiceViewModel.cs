using System;
using System.Collections.Generic;
using System.Text;

namespace BPT_Service.Application.FollowingProviderService.ViewModel
{
    public class FollowingProviderServiceViewModel
    {
        public string UserId { get; set; }
        public string ProviderId { get; set; }
        public DateTime DateCreated { get; set; }
        public DateTime DateModified { get; set; }
        public bool IsReceiveEmail { get; set; }
    }
}
