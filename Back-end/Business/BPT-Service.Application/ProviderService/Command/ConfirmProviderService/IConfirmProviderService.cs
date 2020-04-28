using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace BPT_Service.Application.ProviderService.Command.ConfirmProviderService
{
    public interface IConfirmProviderService
    {
        Task<bool> ExecuteAsync(string secretId);
    }
}
