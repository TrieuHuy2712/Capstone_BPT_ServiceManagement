using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BPT_Service.Application.TagService.ViewModel;
using BPT_Service.Model.Entities;
using BPT_Service.Model.Infrastructure.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace BPT_Service.Application.TagService.Query.GetAllServiceAsync
{
    public class GetAllTagServiceAsyncQuery : IGetAllTagServiceAsyncQuery
    {
        private readonly IRepository<Tag, Guid> _tagRepository;
        public GetAllTagServiceAsyncQuery(IRepository<Tag, Guid> tagRepository)
        {
            _tagRepository = tagRepository;
        }
        public async Task<List<TagViewModel>> ExecuteAsync()
        {
            var listTag = await _tagRepository.FindAllAsync();
            return listTag.Select(x => new TagViewModel
            {
                Id = x.Id.ToString(),
                TagName = x.TagName,
                Description = x.Description,
            }).ToList();
        }
    }
}