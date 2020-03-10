using BPT_Service.Model.Entities;
using BPT_Service.Model.Infrastructure.Interfaces;
using System;
using System.Threading.Tasks;

namespace BPT_Service.Application.EmailService.Command.DeleteEmailService
{
    public class DeleteEmailServiceCommand : IDeleteEmailServiceCommand
    {
        private readonly IRepository<Email, int> _emailRepository;

        public DeleteEmailServiceCommand(IRepository<Email, int> emailRepository)
        {
            _emailRepository = emailRepository;
        }

        public async Task<CommandResult<Email>> ExecuteAsync(int id)
        {
            try
            {
                var idEmail = await _emailRepository.FindByIdAsync(id);
                if (idEmail == null)
                {
                    return new CommandResult<Email>
                    {
                        isValid = false,
                        errorMessage = "Cannot find your id"
                    };
                }
                _emailRepository.Remove(id);
                await _emailRepository.SaveAsync();
                return new CommandResult<Email>
                {
                    isValid = true,
                    myModel = idEmail
                };
            }
            catch (Exception ex)
            {
                return new CommandResult<Email>
                {
                    isValid = false,
                    errorMessage = ex.InnerException.Message.ToString()
                };
            }
        }
    }
}