using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BPT_Service.Application.CommentService.ViewModel;
using BPT_Service.Model.Entities;
using BPT_Service.Model.Entities.ServiceModel;
using BPT_Service.Model.Infrastructure.Interfaces;

namespace BPT_Service.Application.CommentService.Query.GetCommentServiceByIDAsync
{
    public class GetCommentServiceByIDAsyncQuery : IGetCommentServiceByIDAsyncQuery
    {
        private readonly IRepository<ServiceComment, Guid> _commentRepository;
        public GetCommentServiceByIDAsyncQuery(IRepository<ServiceComment, Guid> commentRepository){
            _commentRepository = commentRepository;
        }
        
        public async Task<List<CommentViewModel>> ExecuteAsync(string id)
        {
            var IDProvider = await _commentRepository.FindAllAsync(x=>x.ServiceId==Guid.Parse(id));

            var data = IDProvider.Select(x => new CommentViewModel
            {
                Id = x.Id,
                UserId = x.UserId.ToString(),
                ParentId = x.ParentId,
                ContentOfRating = x.ContentOfRating,
                ServiceId = x.ServiceId.ToString(),
            }).ToList();

            var parentComment = data.Where(x=>x.ParentId == 0).ToList();

            foreach (var item in parentComment)
            {
                var commentChild = data.Where(x=>x.ParentId==item.Id).ToList();
                item.ListVm = commentChild; 
            }
            return parentComment;
        }
    }
}