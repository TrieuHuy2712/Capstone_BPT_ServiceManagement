using System;
using System.Threading.Tasks;
using BPT_Service.Application.TagService.ViewModel;
using BPT_Service.Model.Entities;
using BPT_Service.Model.Infrastructure.Interfaces;

namespace BPT_Service.Application.TagService.Command.AddServiceAsync
{

    public class AddTagServiceAsyncCommand : IAddTagServiceAsyncCommand
    {
        private readonly IRepository<Tag, Guid> _tagRepository;
        public AddTagServiceAsyncCommand(IRepository<Tag, Guid> tagRepository)
        {
            _tagRepository = tagRepository;
        }
        public async Task<bool> ExecuteAsync(TagViewModel userVm)
        {
            Tag tag = new Tag();
            tag.Id = Guid.Parse(userVm.Id);
            tag.TagName = userVm.TagName;
            tag.Description = userVm.Description;
            _tagRepository.Add(tag);
            return true;
        }
    }
}