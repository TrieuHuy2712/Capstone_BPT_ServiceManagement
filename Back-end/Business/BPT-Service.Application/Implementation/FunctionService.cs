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
using BPT_Service.Model.IRepositories;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Http;

namespace BPT_Service.Application.Implementation
{
    public class FunctionService : IFunctionService
    {
        private IRepository<Function, string> _functionRepository;
        private IRepository<Permission, int> _permissionRepository;
        private RoleManager<AppRole> _roleManager;
        private UserManager<AppUser> _userManager;

        private IUnitOfWork _unitOfWork;

        public FunctionService(
            IRepository<Function, string> functionRepository,
            IRepository<Permission, int> permissionRepository,
            RoleManager<AppRole> roleManager,
            IUnitOfWork unitOfWork,
            UserManager<AppUser> userManager)
        {
            _unitOfWork = unitOfWork;
            _functionRepository = functionRepository;
            _permissionRepository = permissionRepository;
            _roleManager = roleManager;
            _userManager = userManager;
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
            function.NameVietNamese = functionVm.NameVietNamese;
            _functionRepository.Add(function);
        }

        public void Delete(string id)
        {
            var getChildItem = _functionRepository.FindAll().Where(x => x.ParentId == id && x.ParentId != null).ToList();
            if (getChildItem.Count() > 0)
            {
                foreach (var item in getChildItem)
                {
                    item.ParentId = null;
                    _functionRepository.Update(item);
                }
            }
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
            function.NameVietNamese = function.NameVietNamese;
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
                NameVietNamese = x.NameVietNamese,
                URL = x.URL
            });
            return queryFunction.ToListAsync();
        }

        public IEnumerable<FunctionViewModel> GetAllWithParentId(string parentId)
        {
            return _functionRepository.FindAll(x => x.ParentId == parentId).Select(x => new FunctionViewModel
            {
                IconCss = x.IconCss,
                Id = x.Id,
                Name = x.Name,
                ParentId = x.ParentId,
                SortOrder = x.SortOrder,
                Status = x.Status,
                NameVietNamese = x.NameVietNamese,
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

            if (functionDb != null)
            {
                functionDb.IconCss = functionVm.IconCss;
                functionDb.Id = functionVm.Id;
                functionDb.Name = functionVm.Name;
                functionDb.ParentId = functionVm.ParentId;
                functionDb.SortOrder = functionVm.SortOrder;
                functionDb.Status = functionVm.Status;
                functionDb.URL = functionVm.URL;
                functionDb.NameVietNamese = functionVm.NameVietNamese;
                _functionRepository.Update(functionDb);
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

        public async Task<List<FunctionViewModel>> GetListFunctionWithPermission(string userName)
        {
            var listPermission = _permissionRepository.FindAll();
            var listRole = _roleManager.Roles.Where(x => x.Name != "admin").ToList();
            var listFunction = _functionRepository.FindAll();
            var getUser = await _userManager.FindByNameAsync(userName);
            var role = await _userManager.GetRolesAsync(getUser);
            List<AppRoleViewModel> listRoleUser = new List<AppRoleViewModel>();
            if (role.Count > 0)
            {
                foreach (var item in role)
                {
                    var roleId = await _roleManager.Roles.Select(x => new AppRoleViewModel
                    {
                        Id = x.Id,
                        Description = x.Description,
                        NameVietNamese = x.NameVietNamese,
                        Name = x.Name
                    }).Where(x => x.Name == item).FirstOrDefaultAsync();
                    listRoleUser.Add(roleId);
                }
            }


            List<FunctionViewModel> functions = new List<FunctionViewModel>();
            List<FunctionViewModel> newFunctions = new List<FunctionViewModel>();
            foreach (var item in listRoleUser)
            {
                var getListFunction = await (from f in listFunction
                                             join p in listPermission on f.Id equals p.FunctionId
                                             where p.RoleId == item.Id && p.CanRead == true
                                             select new FunctionViewModel
                                             {
                                                 Id = f.Id,
                                                 IconCss = f.IconCss,
                                                 Name = f.Name,
                                                 ParentId = f.ParentId,
                                                 SortOrder = f.SortOrder,
                                                 Status = f.Status,
                                                 URL = f.URL,
                                                 NameVietNamese = f.NameVietNamese,
                                             }).ToListAsync();

                functions.AddRange(getListFunction);

            }

            //functions = functions.GroupBy(x => x.Id).Select(t => t.First()).ToList();

            foreach (var item in functions)
            {
                if(functions.Where(x=>x.Name==item.Name).Count()<=1){
                    newFunctions.Add(item);
                }
            }
            return newFunctions;

        }
    }
}