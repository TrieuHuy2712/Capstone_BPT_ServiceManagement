using BPT_Service.Application.EmailService.ViewModel;
using BPT_Service.Common.Dtos;
using BPT_Service.Model.Entities;
using BPT_Service.Model.Infrastructure.Interfaces;
using System.Linq;
using System.Threading.Tasks;

namespace BPT_Service.Application.EmailService.Query.GetAllPagingEmailService
{
    public class GetAllPagingEmailServiceQuery : IGetAllPagingEmailServiceQuery
    {
        private readonly IRepository<Email, int> _emailRepository;

        public GetAllPagingEmailServiceQuery(IRepository<Email, int> emailRepository)
        {
            _emailRepository = emailRepository;
        }

        public async Task<PagedResult<EmailViewModel>> ExecuteAsync(string keyword, int page, int pageSize)
        {
            var query = await _emailRepository.FindAllAsync();
            if (!string.IsNullOrEmpty(keyword))
                query = query.Where(x => x.Name.Contains(keyword)
                || x.Subject.Contains(keyword) || x.Message.Contains(keyword) || x.To.Contains(keyword));

            int totalRow = query.Count();
            query = query.Skip((page - 1) * pageSize)
               .Take(pageSize);

            var data = query.Select(x => new EmailViewModel
            {
                Id = x.Id,
                Message = x.Message,
                To = x.To,
                Subject = x.Subject,
                Name = x.Name
            }).ToList();

            var paginationSet = new PagedResult<EmailViewModel>()
            {
                Results = data,
                CurrentPage = page,
                RowCount = totalRow,
                PageSize = pageSize
            };

            return paginationSet;
        }
    }
}