using System;
using System.Data.SqlClient;
using System.Threading.Tasks;
using MAVN.Persistence.PostgreSQL.Legacy;
using MAVN.Service.Credentials.Domain.Models;
using MAVN.Service.Credentials.Domain.Repositories;
using MAVN.Service.Credentials.MsSqlRepositories.Contexts;
using MAVN.Service.Credentials.MsSqlRepositories.Entities;
using Microsoft.EntityFrameworkCore;
using Npgsql;

namespace MAVN.Service.Credentials.MsSqlRepositories.Repositories
{
    public class PasswordResetRepository : IPasswordResetRepository
    {
        private readonly PostgreSQLContextFactory<CredentialsContext> _contextFactory;
        
        public PasswordResetRepository(PostgreSQLContextFactory<CredentialsContext> contextFactory)
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
                    if (e.InnerException is PostgresException sqlException && 
                        sqlException.SqlState == PostgresErrorCodes.UniqueViolation)
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
