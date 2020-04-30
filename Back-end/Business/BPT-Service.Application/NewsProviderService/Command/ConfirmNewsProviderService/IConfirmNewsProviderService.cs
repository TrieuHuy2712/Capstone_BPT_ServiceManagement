using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace BPT_Service.Application.NewsProviderService.Command.ConfirmNewsProviderService
{
    public interface IConfirmNewsProviderService
    {
        Task<bool> ExecuteAsync(string idCode);
    }
}
