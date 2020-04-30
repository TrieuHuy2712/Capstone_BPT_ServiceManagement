using BPT_Service.Common.Logging;
using BPT_Service.Model.Entities.ServiceModel;
using BPT_Service.Model.Infrastructure.Interfaces;
using System;
using System.Threading.Tasks;

namespace BPT_Service.Application.PostService.Command.ConfirmPostService
{
    public class ConfirmPostService : IConfirmPostService
    {
        private readonly IRepository<Service, Guid> _serviceRepostiroy;

        public ConfirmPostService(IRepository<Service, Guid> serviceRepostiroy)
        {
            _serviceRepostiroy = serviceRepostiroy;
        }

        public async Task<bool> ExecuteAsync(string secretId)
        {
            try
            {
                var splitSecretId = secretId.Split("_");
                var confirmCode = splitSecretId[0];
                var providerId = splitSecretId[1];
                var getInformation = await _serviceRepostiroy.FindSingleAsync(x => x.Id == Guid.Parse(providerId)
                && x.Status == Model.Enums.Status.WaitingApprove && x.codeConfirm == confirmCode);
                if (getInformation != null)
                {
                    getInformation.Status = Model.Enums.Status.Active;
                    _serviceRepostiroy.Update(getInformation);
                    await _serviceRepostiroy.SaveAsync();
                    await Logging<ConfirmPostService>.InformationAsync("Confirmed from " + getInformation.ServiceName);
                    return true;
                }
                await Logging<ConfirmPostService>.ErrorAsync("Cannot find your Id");
                return false;
            }
            catch (Exception ex)
            {
                await Logging<ConfirmPostService>.ErrorAsync(ex.Message.ToString());
                return false;
            }
        }
    }
}