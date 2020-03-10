using BPT_Service.Application.EmailService.ViewModel;
using BPT_Service.Common.Dtos;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace BPT_Service.Application.EmailService.Query.GetAllPagingEmailService
{
    public interface IGetAllPagingEmailServiceQuery
    {
        Task<PagedResult<EmailViewModel>> ExecuteAsync(string keyword, int page, int pageSize);
    }
}
