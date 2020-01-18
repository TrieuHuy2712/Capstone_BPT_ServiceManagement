using BPT_Service.Application.Interfaces;
using BPT_Service.Application.ViewModels.System;
using BPT_Service.Common.Dtos;
using BPT_Service.Model.Entities;
using BPT_Service.Model.Infrastructure.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BPT_Service.Application.Implementation
{
    public class TagService : ITagService
    {
        private readonly IRepository<Tag, int> _tagRepository;
        private readonly IUnitOfWork _unitOfWork;
        public TagService(IRepository<Tag, int> tagRepository, IUnitOfWork unitOfWork)
        {
            _tagRepository = tagRepository;
            _unitOfWork = unitOfWork;
        }

        #region Add
        public async Task<bool> AddAsync(TagViewModel userVm)
        {
            Tag tag = new Tag();
            tag.Id = userVm.Id;
            tag.TagName = userVm.TagName;
            tag.Description = userVm.Description;
            _tagRepository.Add(tag);
            return true;
        }
        #endregion

        #region Delete
        public async Task<bool> DeleteAsync(int id)
        {
            var TagDel = _tagRepository.FindById(id);
            if (TagDel != null)
            {
                _tagRepository.Remove(TagDel);
                return true;
            }
            else
            {
                return false;
            }
        }
        #endregion

        #region Get
        public async Task<List<TagViewModel>> GetAllAsync()
        {
            var listTag = _tagRepository.FindAll();
            var tagViewModel = await listTag.Select(x => new TagViewModel
            {
                Id = x.Id,
                TagName = x.TagName,
                Description = x.Description,
            }).ToListAsync();
            return tagViewModel;
        }

        public PagedResult<TagViewModel> GetAllPagingAsync(string keyword, int page, int pageSize)
        {
            var query = _tagRepository.FindAll();
            if (!string.IsNullOrEmpty(keyword))
                query = query.Where(x => x.TagName.Contains(keyword)
                || x.Description.Contains(keyword));

            int totalRow = query.Count();
            query = query.Skip((page - 1) * pageSize)
               .Take(pageSize);

            var data = query.Select(x => new TagViewModel
            {
                Id = x.Id,
                TagName = x.TagName,
                Description = x.Description
            }).ToList();

            var paginationSet = new PagedResult<TagViewModel>()
            {
                Results = data,
                CurrentPage = page,
                RowCount = totalRow,
                PageSize = pageSize
            };

            return paginationSet;
        }

        public async Task<TagViewModel> GetByID(int id)
        {
            var TagItem = _tagRepository.FindById(id);
            TagViewModel tagViewModels = new TagViewModel();
            tagViewModels.Id = TagItem.Id;
            tagViewModels.TagName = TagItem.TagName;
            tagViewModels.Description = TagItem.Description;
            return tagViewModels;
        }
        #endregion

        #region Update
        public async Task<bool> Update(TagViewModel userVm)
        {
            var TagUpdate = _tagRepository.FindById(userVm.Id);
            if (TagUpdate != null)
            {
                Tag tag = new Tag();
                tag.Id = userVm.Id;
                tag.TagName = userVm.TagName;
                tag.Description = userVm.Description;
                _tagRepository.Update(tag);
                return true;
            }
            else
            {
                return false;
            }
        }

        #endregion

        public void Save()
        {
            _unitOfWork.Commit();
        }
    }
}
