using System.Threading.Tasks;
using AutoMapper;
using Falcon.Common;
using Lykke.Common.MsSql;
using Lykke.Service.Credentials.Domain.Models;
using Lykke.Service.Credentials.Domain.Repositories;
using Lykke.Service.Credentials.MsSqlRepositories.Contexts;
using Lykke.Service.Credentials.MsSqlRepositories.Entities;
using Microsoft.EntityFrameworkCore;

namespace Lykke.Service.Credentials.MsSqlRepositories.Repositories
{
    public class PartnerCredentialsRepository : IPartnerCredentialsRepository
    {
        private readonly MsSqlContextFactory<CredentialsContext> _contextFactory;
        private readonly IMapper _mapper;
        private readonly Sha256HashingUtil _hashingHelper;

        public PartnerCredentialsRepository(
            MsSqlContextFactory<CredentialsContext> contextFactory,
            IMapper mapper,
            Sha256HashingUtil hashingHelper)
        {
            _contextFactory = contextFactory;
            _mapper = mapper;
            _hashingHelper = hashingHelper;
        }

        public async Task<PartnerCredentials> GetByClientIdAsync(string clientId)
        {
            clientId = _hashingHelper.Sha256HashEncoding1252(clientId);
            using (var context = _contextFactory.CreateDataContext())
            {
                var entity = await context.PartnerCredentials.FirstOrDefaultAsync(p => p.ClientId == clientId);

                return _mapper.Map<PartnerCredentials>(entity);
            }
        }

        public async Task<PartnerCredentials> GetByPartnerIdAsync(string partnerId)
        {
            using (var context = _contextFactory.CreateDataContext())
            {
                var entity = await context.PartnerCredentials.FirstOrDefaultAsync(p => p.PartnerId == partnerId);

                return _mapper.Map<PartnerCredentials>(entity);
            }
        }

        public async Task InsertAsync(PartnerCredentials partnerCredentials)
        {
            var entity = _mapper.Map<PartnerCredentialsEntity>(partnerCredentials);
            entity.ClientId = _hashingHelper.Sha256HashEncoding1252(partnerCredentials.ClientId);

            using (var context = _contextFactory.CreateDataContext())
            {
                await context.PartnerCredentials.AddAsync(entity);

                await context.SaveChangesAsync();
            }
        }

        public async Task<bool> DeleteAsync(string clientId)
        {
            clientId = _hashingHelper.Sha256HashEncoding1252(clientId);
            using (var context = _contextFactory.CreateDataContext())
            {
                var entity = await context.PartnerCredentials.FirstOrDefaultAsync(p => p.ClientId == clientId);

                if (entity == null)
                    return false;

                context.PartnerCredentials.Remove(entity);

                await context.SaveChangesAsync();

                return true;
            }
        }

        public async Task UpdateAsync(PartnerCredentials partnerCredentials)
        {
            var entity = _mapper.Map<PartnerCredentialsEntity>(partnerCredentials);
            entity.ClientId = _hashingHelper.Sha256HashEncoding1252(partnerCredentials.ClientId);

            using (var context = _contextFactory.CreateDataContext())
            {
                context.Update(entity);

                await context.SaveChangesAsync();
            }
        }
    }
}
