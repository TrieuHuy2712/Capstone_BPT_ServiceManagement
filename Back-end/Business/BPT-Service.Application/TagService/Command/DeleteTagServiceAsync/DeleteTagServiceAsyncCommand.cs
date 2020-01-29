using System;
using System.Threading.Tasks;
using BPT_Service.Application.TagService.ViewModel;
using BPT_Service.Model.Entities;
using BPT_Service.Model.Infrastructure.Interfaces;

namespace BPT_Service.Application.TagService.Command.DeleteServiceAsync
{
    public class DeleteTagServiceAsyncCommand : IDeleteTagServiceAsyncCommand
    {
        private readonly IRepository<Tag, Guid> _tagRepository;

        public DeleteTagServiceAsyncCommand(IRepository<Tag, Guid> tagRepository)
        {
            _tagRepository = tagRepository;
        }
        public async Task<CommandResult<TagViewModel>> ExecuteAsync(Guid id)
        {
            try
            {
                var TagDel = await _tagRepository.FindByIdAsync(id);
                if (TagDel != null)
                {
                    _tagRepository.Remove(TagDel);
                    await _tagRepository.SaveAsync();
                    return new CommandResult<TagViewModel>
                    {
                        isValid = true,
                        myModel = new TagViewModel
                        {
                            Id = TagDel.Id.ToString(),
                            TagName = TagDel.TagName
                        }
                    };
                }
                else
                {
                    return new CommandResult<TagViewModel>
                    {
                        isValid = false,
                        errorMessage = "Cannot find Tag ID"
                    };
                }
            }
            catch (System.Exception ex)
            {
                return new CommandResult<TagViewModel>
                {
                    isValid = true,
                    errorMessage = ex.InnerException.ToString()
                };
            }
        }
    }
}