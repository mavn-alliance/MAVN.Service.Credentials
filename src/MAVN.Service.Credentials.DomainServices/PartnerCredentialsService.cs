using System.Threading.Tasks;
using Common.Log;
using Common.PasswordTools;
using Lykke.Common.Log;
using MAVN.Service.Credentials.Domain.Exceptions;
using MAVN.Service.Credentials.Domain.Models;
using MAVN.Service.Credentials.Domain.Repositories;
using MAVN.Service.Credentials.Domain.Services;

namespace MAVN.Service.Credentials.DomainServices
{
    public class PartnerCredentialsService : IPartnerCredentialsService
    {
        private readonly IPartnerCredentialsRepository _partnerCredentialsRepository;
        private readonly ILog _log;

        public PartnerCredentialsService(IPartnerCredentialsRepository partnerCredentialsRepository,
            ILogFactory logFactory)
        {
            _partnerCredentialsRepository = partnerCredentialsRepository;
            _log = logFactory.CreateLog(this);
        }

        public async Task CreateAsync(string clientId, string clientSecret, string partnerId)
        {
            var partnerCredentials = await _partnerCredentialsRepository.GetByClientIdAsync(clientId);

            if (partnerCredentials != null)
                throw new PartnerCredentialsAlreadyExistsException();

            partnerCredentials = new PartnerCredentials {ClientId = clientId, PartnerId = partnerId};

            partnerCredentials.SetPassword(clientSecret);

            await _partnerCredentialsRepository.InsertAsync(partnerCredentials);

            _log.Info("Partner credentials created", new {clientId});
        }

        public async Task RemoveAsync(string clientId)
        {
            var result = await _partnerCredentialsRepository.DeleteAsync(clientId);

            if (result)
                _log.Info("Partner credentials removed", new {clientId});
        }

        public async Task UpdateAsync(string clientId, string clientSecret, string partnerId)
        {
            var partnerCredentials = await _partnerCredentialsRepository.GetByPartnerIdAsync(partnerId);

            if (partnerCredentials == null)
                throw new PartnerCredentialsNotFoundException();

            partnerCredentials.ClientId = clientId;
            partnerCredentials.SetPassword(clientSecret);

            await _partnerCredentialsRepository.UpdateAsync(partnerCredentials);

            _log.Info("Partner credentials updated", new { clientId });
        }

        public async Task<(bool isValid, string partnerId)> ValidateAsync(string clientId, string clientSecret)
        {
            var partnerCredentials = await _partnerCredentialsRepository.GetByClientIdAsync(clientId);

            if (partnerCredentials == null)
                throw new PartnerCredentialsNotFoundException();

            return (partnerCredentials.CheckPassword(clientSecret), partnerCredentials.PartnerId);
        }
    }
}
