using System.Threading.Tasks;

namespace BPT_Service.Application.PostService.Query.Extension.GetOwnServiceInformation
{
    public interface IGetOwnServiceInformationQuery
    {
        Task<string> ExecuteAsync(string idService);
    }
}