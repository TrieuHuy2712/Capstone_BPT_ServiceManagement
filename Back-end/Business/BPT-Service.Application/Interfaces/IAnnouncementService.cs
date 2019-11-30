using System;
using BPT_Service.Application.ViewModels.System;
using BPT_Service.Common.Dtos;

namespace BPT_Service.Application.Interfaces
{
    public interface IAnnouncementService
    {
        PagedResult<AnnouncementViewModel> GetAllUnReadPaging(Guid userId, int pageIndex, int pageSize);

        bool MarkAsRead(Guid userId, string id);
    }
}