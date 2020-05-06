using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BPT_Service.Application.CommentService.ViewModel;
using BPT_Service.Model.Entities;
using BPT_Service.Model.Entities.ServiceModel;
using BPT_Service.Model.Infrastructure.Interfaces;
using Microsoft.AspNetCore.Identity;

namespace BPT_Service.Application.CommentService.Query.GetCommentServiceByIDAsync
{
    public class GetCommentServiceByIDAsyncQuery : IGetCommentServiceByIDAsyncQuery
    {
        private readonly IRepository<ServiceComment, int> _commentRepository;
        private readonly UserManager<AppUser> _userManager;

        public GetCommentServiceByIDAsyncQuery(IRepository<ServiceComment, int> commentRepository, UserManager<AppUser> userManager)
        {
            _commentRepository = commentRepository;
            _userManager = userManager;
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
                AvatarPath = _userManager.FindByIdAsync(x.UserId.ToString()).Result.Avatar,
                DateCreated = x.DateCreated,
                UserName = _userManager.FindByIdAsync(x.UserId.ToString()).Result.UserName
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