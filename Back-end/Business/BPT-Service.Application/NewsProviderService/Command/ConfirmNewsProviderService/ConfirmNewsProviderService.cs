using BPT_Service.Common.Logging;
using BPT_Service.Model.Entities.ServiceModel.ProviderServiceModel;
using BPT_Service.Model.Infrastructure.Interfaces;
using System;
using System.Threading.Tasks;

namespace BPT_Service.Application.NewsProviderService.Command.ConfirmNewsProviderService
{
    public class ConfirmNewsProviderService : IConfirmNewsProviderService
    {
        private readonly IRepository<ProviderNew, int> _providerNewsRepostiroy;

        public ConfirmNewsProviderService(IRepository<ProviderNew, int> providerNewsRepostiroy)
        {
            _providerNewsRepostiroy = providerNewsRepostiroy;
        }
        public async Task<bool> ExecuteAsync(string idCode)
        {
            try
            {
                var splitSecretId = idCode.Split("_");
                var confirmCode = splitSecretId[0];
                var providerNewsId = splitSecretId[1];
                var getInformation = await _providerNewsRepostiroy.FindSingleAsync(x => x.Id == int.Parse(providerNewsId)
                && x.Status == Model.Enums.Status.WaitingApprove && x.CodeConfirm == confirmCode);
                if (getInformation != null)
                {
                    getInformation.Status = Model.Enums.Status.Active;
                    _providerNewsRepostiroy.Update(getInformation);
                    await _providerNewsRepostiroy.SaveAsync();
                    await Logging<ConfirmNewsProviderService>.InformationAsync("Confirmed from " + getInformation.Title);
                    return true;
                }
                await Logging<ConfirmNewsProviderService>.ErrorAsync("Cannot find your Id");
                return false;
            }
            catch (Exception ex)
            {
                await Logging<ConfirmNewsProviderService>.ErrorAsync(ex.Message.ToString());
                return false;
            }
        }
    }
}