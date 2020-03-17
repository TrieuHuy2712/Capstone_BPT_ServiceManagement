using BPT_Service.Application.EmailService.ViewModel;
using BPT_Service.Model.Entities;
using BPT_Service.Model.Infrastructure.Interfaces;
using System;
using System.Threading.Tasks;

namespace BPT_Service.Application.EmailService.Command.AddNewEmailService
{
    public class AddNewEmailServiceCommand : IAddNewEmailServiceCommand
    {
        private readonly IRepository<Email, int> _emailRepository;
        public AddNewEmailServiceCommand(IRepository<Email, int> emailRepository)
        {
            _emailRepository = emailRepository;
        }

        public async Task<CommandResult<Email>> ExecuteAsync(EmailViewModel emailVIewModel)
        {
            try
            {
                var mappingEmail = MappingEmail(emailVIewModel);
                await _emailRepository.Add(mappingEmail);
                await _emailRepository.SaveAsync();
                return new CommandResult<Email>
                {
                    isValid = true,
                    myModel = mappingEmail
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

        private Email MappingEmail(EmailViewModel emailViewModel)
        {
            Email email = new Email();
            email.Message = emailViewModel.Message;
            email.Name = emailViewModel.Name;
            email.Subject = emailViewModel.Subject;
            email.To = emailViewModel.To;
            return email;
        }
    }
}
