using System;
using System.Linq;
using System.Threading.Tasks;
using BPT_Service.Application.TagService.ViewModel;
using BPT_Service.Common.Dtos;
using BPT_Service.Model.Entities;
using BPT_Service.Model.Infrastructure.Interfaces;

namespace BPT_Service.Application.TagService.Query.GetAllPagingServiceAsync
{
    public class GetAllPagingTagServiceAsyncQuery : IGetAllPagingTagServiceAsyncQuery
    {
        private readonly IRepository<Tag, Guid> _tagRepository;

        public GetAllPagingTagServiceAsyncQuery(IRepository<Tag, Guid> tagRepository)
        {
            _tagRepository = tagRepository;

        }
        public async Task<PagedResult<TagViewModel>> ExecuteAsync(string keyword, int page, int pageSize)
        {
            var query = await _tagRepository.FindAllAsync();
            if (!string.IsNullOrEmpty(keyword))
                query = query.Where(x => x.TagName.Contains(keyword));

            int totalRow = query.Count();
            query = query.Skip((page - 1) * pageSize)
               .Take(pageSize);

            var data = query.Select(x => new TagViewModel
            {
                Id = x.Id.ToString(),
                TagName = x.TagName,
            }).ToList();

            var paginationSet = new PagedResult<TagViewModel>()
            {
                Results = data,
                CurrentPage = page,
                RowCount = totalRow,
                PageSize = pageSize
            };

            return paginationSet;
        }

    }
}