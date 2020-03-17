using BPT_Service.Application.EmailService.ViewModel;
using BPT_Service.Model.Entities;
using BPT_Service.Model.Infrastructure.Interfaces;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BPT_Service.Application.EmailService.Query.GetAllEmailService
{
    public class GetAllEmailServiceQuery : IGetAllEmailServiceQuery
    {
        private readonly IRepository<Email, int> _emailRepository;
        public GetAllEmailServiceQuery(IRepository<Email, int> emailRepository)
        {
            _emailRepository = emailRepository;
        }

        public async Task<List<EmailViewModel>> ExecuteAsync()
        {
            var findAllEmail = await _emailRepository.FindAllAsync();
            return findAllEmail.Select(x => new EmailViewModel
            {
                Id = x.Id,
                Message = x.Message,
                Name = x.Name,
                Subject = x.Subject,
                To = x.To
            }).ToList();
        }
    }
}
