using System;
using System.Threading.Tasks;
using BPT_Service.Application.TagService.ViewModel;
using BPT_Service.Model.Entities;
using BPT_Service.Model.Infrastructure.Interfaces;

namespace BPT_Service.Application.TagService.Command.UpdateTagServiceAsync
{
    public class UpdateTagServiceAsyncCommand : IUpdateTagServiceAsyncCommand
    {
        private readonly IRepository<Tag, Guid> _tagRepository;
        public UpdateTagServiceAsyncCommand(IRepository<Tag, Guid> tagRepository)
        {
            _tagRepository = tagRepository;
        }
        public async Task<bool> ExecuteAsync(TagViewModel userVm)
        {
            var TagUpdate = await _tagRepository.FindByIdAsync(Guid.Parse(userVm.Id));
            if (TagUpdate != null)
            {
                Tag tag = new Tag();
                tag.Id = Guid.Parse(userVm.Id);
                tag.TagName = userVm.TagName;
                tag.Description = userVm.Description;
                _tagRepository.Update(tag);
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}