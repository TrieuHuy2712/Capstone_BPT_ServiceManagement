using BPT_Service.Application.RatingService.ViewModel;
using BPT_Service.Model.Entities;
using System.Threading.Tasks;

namespace BPT_Service.Application.RatingService.Command.AddRatingService
{
    public interface IAddUpdateRatingServiceCommand
    {
        Task<CommandResult<ServiceRatingViewModel>> ExecuteAsync(ServiceRatingViewModel serviceRatingViewModel);
    }
}