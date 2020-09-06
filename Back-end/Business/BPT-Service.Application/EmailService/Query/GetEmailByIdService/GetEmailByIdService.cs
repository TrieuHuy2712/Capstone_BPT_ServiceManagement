using BPT_Service.Application.EmailService.ViewModel;
using BPT_Service.Model.Entities;
using BPT_Service.Model.Infrastructure.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace BPT_Service.Application.EmailService.Query.GetEmailByIdService
{
    public class GetEmailByIdService : IGetEmailByIdService
    {
        private readonly IRepository<Email, int> _emailRepository;
        public GetEmailByIdService(IRepository<Email, int> emailRepository)
        {
            _emailRepository = emailRepository;
        }

        public async Task<EmailViewModel> ExecuteAsync(int idEmail)
        {
            var findIdEmail = await _emailRepository.FindByIdAsync(idEmail);
            if (findIdEmail == null)
            {
                return null;
            }
            return new EmailViewModel{
                Id = findIdEmail.Id,
                Message = findIdEmail.Message,
                Name = findIdEmail.Name,
                Subject= findIdEmail.Subject,
                To = findIdEmail.To
            };
        }
    }
}
