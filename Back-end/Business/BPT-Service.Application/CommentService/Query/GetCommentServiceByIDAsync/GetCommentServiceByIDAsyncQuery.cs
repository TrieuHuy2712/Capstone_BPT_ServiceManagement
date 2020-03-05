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
        
        public async Task<List<CommentViewModel>> ExecuteAsync(Guid id)
        {
           var IDProvider = await _commentRepository.FindAllAsync(x=>x.ServiceId==Guid.Parse(id));

            var data = IDProvider.Select(x => new CommentViewModel
            {
                Id = x.Id.ToString(),
                UserId = x.UserId.ToString(),
                ParentId = x.ParentId.ToString(),
                ContentOfRating = x.ContentOfRating,
                ServiceId = x.ServiceId.ToString(),
            }).ToList();

            var parentComment = data.Where(x=>x.ParentId == null).ToList();

            foreach (var item in parentComment)
            {
                var commentChild = data.Where(x=>x.ParentId==item.Id).ToList();
                item.ListVm = commentChild; 
            }
            // CommentViewModel commentViewModels = new CommentViewModel();
            // commentViewModels.Id = IDProvider.Id.ToString();
            // commentViewModels.UserId = IDProvider.UserId.ToString();
            // commentViewModels.ParentId = IDProvider.ParentId.ToString();
            // commentViewModels.ContentOfRating = IDProvider.ContentOfRating;
            return parentComment;
        }
    }
}