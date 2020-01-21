using System;
using System.Threading.Tasks;
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
        public async Task<bool> ExecuteAsync(Guid id)
        {
            var TagDel = await _tagRepository.FindByIdAsync(id);
            if (TagDel != null)
            {
                _tagRepository.Remove(TagDel);
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}