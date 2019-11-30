using AutoMapper;
using BPT_Service.Application.ViewModels.System;
using BPT_Service.Model.Entities;

namespace BPT_Service.Application.AutoMapper
{
    public class DomainToViewModelMappingProfile:Profile
    {
         public DomainToViewModelMappingProfile()
        {

            CreateMap<Announcement, AnnouncementViewModel>().MaxDepth(2);
            CreateMap<Function, FunctionViewModel>();
            CreateMap<AppUser, AppUserViewModel>();
            CreateMap<AppRole, AppRoleViewModel>();

        }
    }
}