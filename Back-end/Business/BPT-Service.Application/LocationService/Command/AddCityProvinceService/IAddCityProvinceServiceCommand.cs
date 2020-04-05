using BPT_Service.Application.LocationService.ViewModel;
using BPT_Service.Model.Entities;
using System.Threading.Tasks;

namespace BPT_Service.Application.LocationService.Command.AddCityProvinceService
{
    public interface IAddCityProvinceServiceCommand
    {
        Task<CommandResult<CityProvinceViewModel>> ExecuteAsync(CityProvinceViewModel listAddViewModel);
    }
}