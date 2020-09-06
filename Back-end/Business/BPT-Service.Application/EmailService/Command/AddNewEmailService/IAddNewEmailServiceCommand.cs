using BPT_Service.Application.EmailService.ViewModel;
using BPT_Service.Model.Entities;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace BPT_Service.Application.EmailService.Command.AddNewEmailService
{
    public interface IAddNewEmailServiceCommand
    {
        Task<CommandResult<Email>> ExecuteAsync(EmailViewModel emailVIewModel);
    }
}
