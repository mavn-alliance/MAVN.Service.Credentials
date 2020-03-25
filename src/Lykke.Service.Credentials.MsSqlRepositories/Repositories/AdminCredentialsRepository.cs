using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Falcon.Common;
using Lykke.Common.MsSql;
using Lykke.Service.Credentials.Domain.Models;
using Lykke.Service.Credentials.Domain.Repositories;
using Lykke.Service.Credentials.MsSqlRepositories.Contexts;
using Lykke.Service.Credentials.MsSqlRepositories.Entities;

namespace Lykke.Service.Credentials.MsSqlRepositories.Repositories
{
    public class AdminCredentialsRepository : IAdminCredentialsRepository
    {
        private readonly MsSqlContextFactory<CredentialsContext> _contextFactory;
        private readonly IMapper _mapper;
        private readonly Sha256HashingUtil _hashingHelper;

        public AdminCredentialsRepository(
            MsSqlContextFactory<CredentialsContext> contextFactory,
            IMapper mapper,
            Sha256HashingUtil hashingHelper)
        {
            _contextFactory = contextFactory;
            _mapper = mapper;
            _hashingHelper = hashingHelper;
        }

        public async Task<AdminCredentials> GetByLoginAsync(string login)
        {
            var normalizedLogin = NormalizeLogin(login);
            login = _hashingHelper.Sha256HashEncoding1252(login);
            using (var context = _contextFactory.CreateDataContext())
            {
                var entity =
                    await context.AdminCredentials.FindAsync(login) ??
                    await context.AdminCredentials.FindAsync(_hashingHelper.Sha256HashEncoding1252(normalizedLogin));

                return _mapper.Map<AdminCredentials>(entity);
            }
        }

        public async Task InsertAsync(AdminCredentials adminCredentials)
        {
            var entity = _mapper.Map<AdminCredentialsEntity>(adminCredentials);
            entity.Login = _hashingHelper.Sha256HashEncoding1252(NormalizeLogin(entity.Login));

            using (var context = _contextFactory.CreateDataContext())
            {
                await context.AdminCredentials.AddAsync(entity);

                await context.SaveChangesAsync();
            }
        }

        public async Task UpdateAsync(AdminCredentials adminCredentials)
        {
            var entity = _mapper.Map<AdminCredentialsEntity>(adminCredentials);
            entity.Login = _hashingHelper.Sha256HashEncoding1252(NormalizeLogin(entity.Login));

            using (var context = _contextFactory.CreateDataContext())
            {
                context.Update(entity);

                await context.SaveChangesAsync();
            }
        }

        public async Task<bool> DeleteAsync(string login)
        {
            var normalizedLogin = NormalizeLogin(login);
            login = _hashingHelper.Sha256HashEncoding1252(login);

            using (var context = _contextFactory.CreateDataContext())
            {
                var entity =
                    await context.AdminCredentials.FindAsync(login) ??
                    await context.AdminCredentials.FindAsync(_hashingHelper.Sha256HashEncoding1252(normalizedLogin));

                if (entity == null)
                    return false;

                context.AdminCredentials.RemoveRange(context.AdminCredentials.Where(o => o.Login == login));

                await context.SaveChangesAsync();

                return true;
            }
        }

        private string NormalizeLogin(string login)
        {
            return login.ToLower();
        }
    }
}
