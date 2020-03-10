using BPT_Service.Model.Entities;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace BPT_Service.Application.EmailService.Command.DeleteEmailService
{
    public interface IDeleteEmailServiceCommand
    {
        Task<CommandResult<Email>> ExecuteAsync(int id);
    }
}
