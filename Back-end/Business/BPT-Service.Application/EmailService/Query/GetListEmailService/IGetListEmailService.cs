using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace BPT_Service.Application.EmailService.Query.GetListEmailService
{
    public interface IGetListEmailService
    {
        Task<List<string>> ExecuteAsync();
    }
}
