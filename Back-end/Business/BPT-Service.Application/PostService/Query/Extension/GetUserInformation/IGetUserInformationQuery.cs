using BPT_Service.Model.Entities.ServiceModel;
using System;
using System.Collections.Generic;

namespace BPT_Service.Application.PostService.Query.Extension.GetUserInformation
{
    public interface IGetUserInformationQuery
    {
        string ExecuteAsync(Guid idService, IEnumerable<Service> services,
            IEnumerable<Model.Entities.ServiceModel.UserServiceModel.UserService> userService);
    }
}