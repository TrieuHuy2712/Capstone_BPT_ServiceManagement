using BPT_Service.Model.Entities;
using BPT_Service.Model.Entities.ServiceModel;
using System.Threading.Tasks;

namespace BPT_Service.Application.RatingService.Command.DeleteRatingService
{
    public interface IDeleteRatingServiceCommand
    {
        Task<CommandResult<ServiceRating>> ExecuteAsync(int idRating);
    }
}