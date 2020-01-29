using System;
using System.Threading.Tasks;
using BPT_Service.Application.TagService.ViewModel;
using BPT_Service.Model.Entities;
using BPT_Service.Model.Infrastructure.Interfaces;

namespace BPT_Service.Application.TagService.Query.GetByIDTagServiceAsync
{
    public class GetByIDTagServiceAsyncQuery : IGetByIDTagServiceAsyncQuery
    {
        private readonly IRepository<Tag, Guid> _tagRepository;
        public GetByIDTagServiceAsyncQuery(IRepository<Tag, Guid> tagRepository){
            _tagRepository = tagRepository;
        }
        
        public async Task<TagViewModel> ExecuteAsync(Guid id)
        {
           var TagItem = await _tagRepository.FindByIdAsync(id);
            TagViewModel tagViewModels = new TagViewModel();
            tagViewModels.Id = TagItem.Id.ToString();
            tagViewModels.TagName = TagItem.TagName;
            return tagViewModels;
        }
    }
}