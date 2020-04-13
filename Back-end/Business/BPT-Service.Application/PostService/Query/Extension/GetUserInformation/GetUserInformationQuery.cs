using BPT_Service.Model.Entities;
using BPT_Service.Model.Entities.ServiceModel;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;

namespace BPT_Service.Application.PostService.Query.Extension.GetUserInformation
{
    public class GetUserInformationQuery : IGetUserInformationQuery
    {
        private readonly UserManager<AppUser> _userManager;

        public GetUserInformationQuery(UserManager<AppUser> userManager)
        {
            _userManager = userManager;
        }

        public string ExecuteAsync(Guid idService, IEnumerable<Service> services, IEnumerable<Model.Entities.ServiceModel.UserServiceModel.UserService> userService)
        {
            var serv = services.Where(x => x.Id == idService).FirstOrDefault();
            if (serv == null)
            {
                return "";
            }
            var findUser = userService.Where(x => x.ServiceId == serv.Id).FirstOrDefault();
            if (findUser == null)
            {
                return "";
            }
            var getUser = _userManager.FindByIdAsync(findUser.UserId.ToString()).Result;
            if (getUser != null)
            {
                return getUser.UserName + "(" + getUser.Email + ")";
            }
            return "";
        }
    }
}