using System;
using System.Data.SqlClient;
using System.Threading.Tasks;
using Lykke.Common.MsSql;
using Lykke.Service.Credentials.Domain.Models;
using Lykke.Service.Credentials.Domain.Repositories;
using Lykke.Service.Credentials.MsSqlRepositories.Contexts;
using Lykke.Service.Credentials.MsSqlRepositories.Entities;
using Microsoft.EntityFrameworkCore;

namespace Lykke.Service.Credentials.MsSqlRepositories.Repositories
{
    public class PasswordResetRepository : IPasswordResetRepository
    {
        private readonly MsSqlContextFactory<CredentialsContext> _contextFactory;
        
        private const int PrimaryKeyViolationErrorCode = 2627;
        
        public PasswordResetRepository(MsSqlContextFactory<CredentialsContext> contextFactory)
        {
            _contextFactory = contextFactory;
        }
       
        public async Task CreateOrUpdateIdentifierAsync(string customerId, string identifier, TimeSpan identifierTimeSpan)
        {
            var entity = PasswordResetEntity.Create(customerId, identifier, identifierTimeSpan);

            using (var context = _contextFactory.CreateDataContext())
            {
                await context.AddAsync(entity);

                try
                {
                    await context.SaveChangesAsync();
                }
                catch (DbUpdateException e)
                {
                    if (e.InnerException is SqlException sqlException && sqlException.Number ==
                        PrimaryKeyViolationErrorCode)
                    {
                        context.PasswordReset.Update(entity);

                        await context.SaveChangesAsync();
                    }
                    else throw;
                }
            }
        }

        public async Task<IResetIdentifier> GetIdentifierAsync(string customerId)
        {
            using (var context = _contextFactory.CreateDataContext())
            {
                var entity = await context.PasswordReset.FindAsync(customerId);

                return entity;
            }
        }

        public async Task<IResetIdentifier> GetByIdentifierAsync(string resetIdentifier)
        {
            using (var context = _contextFactory.CreateDataContext())
            {
                var entity = await context.PasswordReset
                    .FirstOrDefaultAsync(i => i.Identifier == resetIdentifier);

                return entity;
            }
        }

        public async Task RemoveAsync(string customerId)
        {
            using (var context = _contextFactory.CreateDataContext())
            {
                var entity = new PasswordResetEntity {CustomerId = customerId};

                context.PasswordReset.Attach(entity);

                context.Remove(entity);

                await context.SaveChangesAsync();
            }
        }
    }
}
