using BPT_Service.Application.EmailService.ViewModel;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace BPT_Service.Application.EmailService.Query.GetEmailByIdService
{
    public interface IGetEmailByIdService
    {
        Task<EmailViewModel> ExecuteAsync(int idEmail);
    }
}
