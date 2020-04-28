using BPT_Service.Common.Logging;
using BPT_Service.Model.Entities.ServiceModel;
using BPT_Service.Model.Infrastructure.Interfaces;
using System;
using System.Threading.Tasks;

namespace BPT_Service.Application.ProviderService.Command.ConfirmProviderService
{
    public class ConfirmProviderService : IConfirmProviderService
    {
        private readonly IRepository<Provider, Guid> _providerRepostiroy;

        public ConfirmProviderService(IRepository<Provider, Guid> providerRepostiroy)
        {
            _providerRepostiroy = providerRepostiroy;
        }

        public async Task<bool> ExecuteAsync(string secretId)
        {
            try
            {
                var splitSecretId = secretId.Split("_");
                var confirmCode = splitSecretId[0];
                var providerId = splitSecretId[1];
                var getInformation = await _providerRepostiroy.FindSingleAsync(x => x.Id == Guid.Parse(providerId) && x.OTPConfirm == confirmCode);
                if (getInformation != null)
                {
                    getInformation.Status = Model.Enums.Status.Active;
                    _providerRepostiroy.Update(getInformation);
                    await _providerRepostiroy.SaveAsync();
                    await Logging<ConfirmProviderService>.InformationAsync("Confirmed from " + getInformation.ProviderName);
                    return true;
                }
                await Logging<ConfirmProviderService>.ErrorAsync("Cannot find your Id");
                return false;
            }
            catch (Exception ex)
            {
                await Logging<ConfirmProviderService>.ErrorAsync(ex.Message.ToString());
                return false;
            }
        }
    }
}