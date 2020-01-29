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
        public async Task<CommandResult<TagViewModel>> ExecuteAsync(TagViewModel userVm)
        {
            try
            {
                Tag tag = new Tag();
                tag.TagName = userVm.TagName;
                await _tagRepository.Add(tag);
                await _tagRepository.SaveAsync();
                return new CommandResult<TagViewModel>
                {
                    isValid = true,
                    myModel = new TagViewModel
                    {
                        Id = tag.Id.ToString(),
                        TagName = tag.TagName
                    }
                };
            }
            catch (System.Exception ex)
            {
                return new CommandResult<TagViewModel>
                {
                    isValid = false,
                    errorMessage = ex.InnerException.ToString()
                };
            }
        }
    }
}