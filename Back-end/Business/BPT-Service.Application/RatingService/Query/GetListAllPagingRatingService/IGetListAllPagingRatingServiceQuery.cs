﻿using BPT_Service.Application.RatingService.ViewModel;
using BPT_Service.Common.Dtos;
using System.Threading.Tasks;

namespace BPT_Service.Application.RatingService.Query.GetListAllPagingRatingService
{
    public interface IGetListAllPagingRatingServiceQuery
    {
        Task<PagedResult<ListRatingByServiceViewModel>> ExecuteAsync(string keyword, int page, int pageSize);
    }
}