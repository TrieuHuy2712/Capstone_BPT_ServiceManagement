using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using BPT_Service.Application.CommentService.ViewModel;

namespace BPT_Service.Application.CommentService.Query.GetCommentServiceByIDAsync
{
    public interface IGetCommentServiceByIDAsyncQuery
    {
          Task<List<CommentViewModel>> ExecuteAsync(string id);
    }
}