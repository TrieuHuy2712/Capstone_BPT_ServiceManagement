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
        public async Task<CommandResult<TagViewModel>> ExecuteAsync(TagViewModel userVm)
        {
            try
            {
                var TagUpdate = await _tagRepository.FindByIdAsync(Guid.Parse(userVm.Id));
                if (TagUpdate != null)
                {
                    Tag tag = new Tag();
                    tag.Id = Guid.Parse(userVm.Id);
                    tag.TagName = userVm.TagName;
                    _tagRepository.Update(tag);
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
                else
                {
                    return new CommandResult<TagViewModel>
                    {
                        isValid = false,
                        errorMessage = "Cannot find Tag"
                    };
                }
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