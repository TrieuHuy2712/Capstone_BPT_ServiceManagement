using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace BPT_Service.Application.PostService.Command.ConfirmPostService
{
    public interface IConfirmPostService
    {
        Task<bool> ExecuteAsync(string secretId);
    }
}
