using System;
using System.Threading.Tasks;
using Common;
using Common.Log;
using Common.PasswordTools;
using MAVN.Common;
using Lykke.Common.Log;
using MAVN.Common.MsSql;
using MAVN.Service.Credentials.Domain.Models;
using MAVN.Service.Credentials.Domain.Repositories;
using MAVN.Service.Credentials.MsSqlRepositories.Contexts;
using MAVN.Service.Credentials.MsSqlRepositories.Entities;
using Microsoft.EntityFrameworkCore;

namespace MAVN.Service.Credentials.MsSqlRepositories.Repositories
{
    public class CustomerCredentialsRepository : ICustomerCredentialsRepository
    {
        private readonly MsSqlContextFactory<CredentialsContext> _contextFactory;
        private readonly Sha256HashingUtil _hashingHelper;
        private readonly ILog _log;

        public CustomerCredentialsRepository(
            MsSqlContextFactory<CredentialsContext> contextFactory,
            ILogFactory logFactory,
            Sha256HashingUtil hashingHelper)
        {
            _contextFactory = contextFactory;
            _hashingHelper = hashingHelper;
            _log = logFactory.CreateLog(this);
        }
        
        public async Task CreateAsync(
            string customerId,
            string login,
            string password)
        {
            login = NormalizeLogin(login);
            login = _hashingHelper.Sha256HashEncoding1252(login);
            var entity = CustomerCredentialsEntity.Create(
                customerId,
                login,
                password);

            using (var context = _contextFactory.CreateDataContext())
            {
                await context.CustomerCredentials.AddAsync(entity);

                await context.SaveChangesAsync();
            }
        }

        public async Task<IHashedCustomerCredentials> GetAsync(string login)
        {
            var normalizedLogin = NormalizeLogin(login);
            var hashedLogin = _hashingHelper.Sha256HashEncoding1252(normalizedLogin);
            using (var context = _contextFactory.CreateDataContext())
            {
                var entity = await context.CustomerCredentials.FindAsync(hashedLogin);
                if (entity != null)
                    return entity;

                var oldHashedLogin = _hashingHelper.Sha256HashEncoding1252(login);
                entity = await context.CustomerCredentials.FindAsync(oldHashedLogin);
                if (entity == null)
                    return null;

                context.CustomerCredentials.Remove(entity);
                var copy = entity.Copy();
                copy.Login = normalizedLogin;
                context.CustomerCredentials.Add(copy);

                await context.SaveChangesAsync();

                return entity;
            }
        }

        public async Task<bool> RemoveAsync(string login)
        {
            var normalizedLogin = NormalizeLogin(login);
            var hashedLogin = _hashingHelper.Sha256HashEncoding1252(normalizedLogin);
            using (var context = _contextFactory.CreateDataContext())
            {
                try
                {
                    var entity = await context.CustomerCredentials.FindAsync(hashedLogin);
                    if (entity == null)
                    {
                        var oldHashedLogin = _hashingHelper.Sha256HashEncoding1252(login);
                        entity = await context.CustomerCredentials.FindAsync(oldHashedLogin);
                    }

                    if (entity == null)
                        return false;

                    context.CustomerCredentials.Remove(entity);

                    await context.SaveChangesAsync();

                    return true;
                }
                catch (Exception e)
                {
                    _log.Error(e, "Couldn't remove customer credentials", new { login = login.SanitizeEmail() });

                    return false;
                }
            }
        }

        public async Task UpdatePasswordAsync(string login, string password)
        {
            var normalizedLogin = NormalizeLogin(login);
            var hashedLogin = _hashingHelper.Sha256HashEncoding1252(normalizedLogin);
            using (var context = _contextFactory.CreateDataContext())
            {
                var entity = await context.CustomerCredentials.FindAsync(hashedLogin);
                if (entity == null)
                {
                    var oldHashedLogin = _hashingHelper.Sha256HashEncoding1252(login);
                    entity = await context.CustomerCredentials.FindAsync(oldHashedLogin);
                }
                if (entity == null)
                    throw new InvalidOperationException($"Couldn't find a login '{login.SanitizeEmail()}' to update");

                if (entity.Login != hashedLogin)
                {
                    context.CustomerCredentials.Remove(entity);
                    var copy = entity.Copy();
                    copy.Login = normalizedLogin;
                    copy.SetPassword(password);
                    context.CustomerCredentials.Add(copy);
                }
                else
                {
                    entity.SetPassword(password);
                }

                context.CustomerCredentials.Update(entity);

                await context.SaveChangesAsync();
            }
        }

        public async Task<bool> UpdateLoginAsync(string oldLogin, string newLogin)
        {
            var normalizedLogin = NormalizeLogin(oldLogin);
            var hashedLogin = _hashingHelper.Sha256HashEncoding1252(normalizedLogin);
            using (var context = _contextFactory.CreateDataContext())
            {
                try
                {
                    using (var transaction = context.Database.BeginTransaction())
                    {
                        var entity = await context.CustomerCredentials.FindAsync(hashedLogin);
                        if (entity == null)
                        {
                            var oldHashedLogin = _hashingHelper.Sha256HashEncoding1252(oldLogin);
                            entity = await context.CustomerCredentials.FindAsync(oldHashedLogin);
                        }
                        if (entity == null)
                            throw new InvalidOperationException($"Couldn't find a login '{oldLogin.SanitizeEmail()}' to update");

                        context.CustomerCredentials.Remove(entity);

                        var copy = entity.Copy();
                        newLogin = NormalizeLogin(newLogin);
                        copy.Login = _hashingHelper.Sha256HashEncoding1252(newLogin);
                        context.CustomerCredentials.Add(copy);

                        await context.SaveChangesAsync();

                        transaction.Commit();

                        return true;
                    }
                }
                catch (DbUpdateConcurrencyException ex)
                {
                    _log.Warning(null, ex);
                    return false;
                }
            }
        }

        private string NormalizeLogin(string login)
        {
            return login.ToLower();
        }

        public async Task SetPinAsync(string customerId, string pinCode)
        {
            using (var context = _contextFactory.CreateDataContext())
            {
                var entity = await context.CustomerCredentials
                    .Include(c => c.PinCode)
                    .FirstOrDefaultAsync(c => c.CustomerId == customerId);

                entity.PinCode = PinCodeEntity.Create(customerId, pinCode);

                context.Update(entity);

                await context.SaveChangesAsync();
            }
        }

        public async Task<IPasswordKeeping> GetPinByCustomerIdAsync(string customerId)
        {
            using (var context = _contextFactory.CreateDataContext())
            {
                var entity = await context.CustomerPinCodes.FirstOrDefaultAsync(c => c.CustomerId == customerId);

                return entity;
            }
        }

        public async Task<IHashedCustomerCredentials> GetByCustomerIdAsync(string customerId)
        {
            using (var context = _contextFactory.CreateDataContext())
            {
                var entity = await context.CustomerCredentials.FirstOrDefaultAsync(c => c.CustomerId == customerId);

                return entity;
            }
        }
    }
}
