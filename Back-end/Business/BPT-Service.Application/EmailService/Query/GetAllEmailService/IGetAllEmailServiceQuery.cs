using BPT_Service.Application.EmailService.ViewModel;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace BPT_Service.Application.EmailService.Query.GetAllEmailService
{
    public interface IGetAllEmailServiceQuery
    {
        Task<List<EmailViewModel>> ExecuteAsync();
    }
}
