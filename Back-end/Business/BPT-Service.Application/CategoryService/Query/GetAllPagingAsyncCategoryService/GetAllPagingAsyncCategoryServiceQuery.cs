using System.Linq;
using System.Threading.Tasks;
using BPT_Service.Application.CategoryService.ViewModel;
using BPT_Service.Common.Dtos;
using BPT_Service.Model.Entities;
using BPT_Service.Model.Infrastructure.Interfaces;

namespace BPT_Service.Application.CategoryService.Query.GetAllPagingAsyncCategoryService
{
    public class GetAllPagingAsyncCategoryServiceQuery : IGetAllPagingAsyncCategoryServiceQuery
    {
        private readonly IRepository<Category, int> _categoryRepository;
        public GetAllPagingAsyncCategoryServiceQuery(IRepository<Category, int> categoryRepository, IUnitOfWork unitOfWork)
        {
            _categoryRepository = categoryRepository;
        }
        public async Task<PagedResult<CategoryServiceViewModel>> ExecuteAsync(string keyword, int page, int pageSize)
        {
            var query = await _categoryRepository.FindAllAsync();
            if (!string.IsNullOrEmpty(keyword))
                query = query.Where(x => x.CategoryName.Contains(keyword)
                || x.Description.Contains(keyword));

            int totalRow = query.Count();
            query = query.Skip((page - 1) * pageSize)
               .Take(pageSize);

            var data = query.Select(x => new CategoryServiceViewModel
            {
                Id = x.Id,
                CategoryName = x.CategoryName,
                Description = x.Description,
            }).ToList();

            var paginationSet = new PagedResult<CategoryServiceViewModel>()
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