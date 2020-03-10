using System;

namespace BPT_Service.Application.FollowingProviderService.ViewModel
{
    public class UserFollowingByProviderViewModel
    {
        public string UserId { get; set; }
        public string ProviderId { get; set; }
        public DateTime DateCreated { get; set; }
        public DateTime DateModified { get; set; }
        public bool IsReceiveEmail { get; set; }
        public string UserName { get; set; }
    }
}