using BPT_Service.Application.EmailService.ViewModel;
using BPT_Service.Model.Entities;
using BPT_Service.Model.Infrastructure.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace BPT_Service.Application.EmailService.Command.UpdateNewEmailService
{
    public class UpdateNewEmailServiceCommand : IUpdateNewEmailServiceCommand
    {
        private readonly IRepository<Email, int> _emailRepository;
        public UpdateNewEmailServiceCommand(IRepository<Email, int> emailRepository)
        {
            _emailRepository = emailRepository;
        }

        public async Task<CommandResult<Email>> ExecuteAsync(EmailViewModel emailViewModel)
        {
            try
            {
                var checkId = await _emailRepository.FindByIdAsync(emailViewModel.Id);
                if (checkId == null)
                {
                    return new CommandResult<Email>
                    {
                        isValid = false,
                        errorMessage = "Cannot find your Id"
                    };
                }
                var mappingEmail = MappingEmail(checkId, emailViewModel);
                _emailRepository.Update(mappingEmail);
                await _emailRepository.SaveAsync();
                return new CommandResult<Email>
                {
                    isValid = true,
                    myModel = checkId
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
        private Email MappingEmail(Email email, EmailViewModel emailViewModel)
        {
            email.Message = emailViewModel.Message;
            email.Name = emailViewModel.Name;
            email.Subject = emailViewModel.Subject;
            email.To = emailViewModel.To;
            return email;
        }
    }
}
