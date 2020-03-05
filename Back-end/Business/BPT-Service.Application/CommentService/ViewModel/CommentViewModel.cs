using System.Collections.Generic;

namespace BPT_Service.Application.CommentService.ViewModel
{
    public class CommentViewModel
    {
        public string Id { get; set; }

        public string UserId { get; set; }

        public string ServiceId { get; set; }
        
        public string ParentId { get; set; }

        public string ContentOfRating { get; set; }

        public List<CommentViewModel> ListVm {get;set;}

    }
}