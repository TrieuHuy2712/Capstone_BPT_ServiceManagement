using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using BPT_Service.Application.Interfaces;
using BPT_Service.Application.ViewModels.System;
using BPT_Service.Model.Entities;
using BPT_Service.Model.Enums;
using BPT_Service.Model.Infrastructure.Interfaces;

namespace BPT_Service.Application.Implementation
{
    public class FunctionService : IFunctionService
    {
        private IRepository<Function, string> _functionRepository;
        private IUnitOfWork _unitOfWork;

        public FunctionService(
            IRepository<Function, string> functionRepository,
            IUnitOfWork unitOfWork)
        {
            _functionRepository = functionRepository;
            _unitOfWork = unitOfWork;
        }

        public bool CheckExistedId(string id)
        {
            return _functionRepository.FindById(id) != null;
        }

        public void Add(FunctionViewModel functionVm)
        {
            //var function = _mapper.Map<Function>(functionVm);
            Function function = new Function();
            function.Id = functionVm.Id;
            function.IconCss = functionVm.IconCss;
            function.Name = functionVm.Name;
            function.ParentId = functionVm.ParentId;
            function.SortOrder = functionVm.SortOrder;
            function.Status = functionVm.Status;
            function.URL = functionVm.URL;
            _functionRepository.Add(function);
        }

        public void Delete(string id)
        {
            _functionRepository.Remove(id);
        }

        public FunctionViewModel GetById(string id)
        {
            var function = _functionRepository.FindSingle(x => x.Id == id);
            //return _mapper.Map<Function, FunctionViewModel>(function);
            FunctionViewModel functionViewModel = new FunctionViewModel();
            functionViewModel.IconCss = function.IconCss;
            functionViewModel.Id = function.Id;
            functionViewModel.Name = function.Name;
            functionViewModel.ParentId = function.ParentId;
            functionViewModel.SortOrder = function.SortOrder;
            functionViewModel.Status = function.Status;
            functionViewModel.URL = function.URL;
            return functionViewModel;
        }

        public Task<List<FunctionViewModel>> GetAll(string filter)
        {
            var query = _functionRepository.FindAll(x => x.Status == Status.Active);
            if (!string.IsNullOrEmpty(filter))
                query = query.Where(x => x.Name.Contains(filter));
            var queryFunction = query.OrderBy(x => x.ParentId).Select(x => new FunctionViewModel
            {
                IconCss = x.IconCss,
                Id = x.Id,
                Name = x.Name,
                ParentId = x.ParentId,
                SortOrder = x.SortOrder,
                Status = x.Status,
                URL = x.URL
            });
            return queryFunction.ToListAsync();
        }

        public IEnumerable<FunctionViewModel> GetAllWithParentId(string parentId)
        {
            //return _functionRepository.FindAll(x => x.ParentId == parentId).ProjectTo<FunctionViewModel>(_config);
            return _functionRepository.FindAll(x => x.ParentId == parentId).Select(x=>new FunctionViewModel{
                IconCss = x.IconCss,
                Id = x.Id,
                Name = x.Name,
                ParentId = x.ParentId,
                SortOrder = x.SortOrder,
                Status = x.Status,
                URL = x.URL
            });
        }

        public void Save()
        {
            _unitOfWork.Commit();
        }

        public void Update(FunctionViewModel functionVm)
        {
            var functionDb = _functionRepository.FindById(functionVm.Id);
            //var function = _mapper.Map<Function>(functionVm);
            if (functionDb != null)
            {
                Function function = new Function();
                function.IconCss = functionVm.IconCss;
                function.Id = functionVm.Id;
                function.Name = functionVm.Name;
                function.ParentId = functionVm.ParentId;
                function.SortOrder = functionVm.SortOrder;
                function.Status = functionVm.Status;
                function.URL = functionVm.URL;
                _functionRepository.Update(function);
            }

        }

        public void ReOrder(string sourceId, string targetId)
        {
            var source = _functionRepository.FindById(sourceId);
            var target = _functionRepository.FindById(targetId);
            int tempOrder = source.SortOrder;

            source.SortOrder = target.SortOrder;
            target.SortOrder = tempOrder;

            _functionRepository.Update(source);
            _functionRepository.Update(target);
        }

        public void UpdateParentId(string sourceId, string targetId, Dictionary<string, int> items)
        {
            //Update parent id for source
            var category = _functionRepository.FindById(sourceId);
            category.ParentId = targetId;
            _functionRepository.Update(category);

            //Get all sibling
            var sibling = _functionRepository.FindAll(x => items.ContainsKey(x.Id));
            foreach (var child in sibling)
            {
                child.SortOrder = items[child.Id];
                _functionRepository.Update(child);
            }
        }

        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }
    }
}