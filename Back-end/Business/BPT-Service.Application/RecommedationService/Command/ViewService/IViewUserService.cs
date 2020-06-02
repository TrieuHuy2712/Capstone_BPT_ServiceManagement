using BPT_Service.Model;
using BPT_Service.Model.Entities;
using System.Threading.Tasks;

namespace BPT_Service.Application.RecommedationService.Command.ViewService
{
    public interface IViewUserService
    {
        Task<CommandResult<UserRecommendation>> ExecuteAsync(string idService);
    }
}