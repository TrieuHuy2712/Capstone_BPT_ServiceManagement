using BPT_Service.Application.EmailService.ViewModel;
using BPT_Service.Model.Entities;
using System.Threading.Tasks;

namespace BPT_Service.Application.EmailService.Command.UpdateNewEmailService
{
    public interface IUpdateNewEmailServiceCommand
    {
        Task<CommandResult<Email>> ExecuteAsync(EmailViewModel emailViewModel);
    }
}