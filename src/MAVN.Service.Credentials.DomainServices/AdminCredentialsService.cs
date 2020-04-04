using System;
using System.Threading.Tasks;
using Common;
using Common.Log;
using Common.PasswordTools;
using Lykke.Common.Log;
using MAVN.Service.Credentials.Domain.Exceptions;
using MAVN.Service.Credentials.Domain.Models;
using MAVN.Service.Credentials.Domain.Repositories;
using MAVN.Service.Credentials.Domain.Services;

namespace MAVN.Service.Credentials.DomainServices
{
    public class AdminCredentialsService : IAdminCredentialsService
    {
        private readonly IAdminCredentialsRepository _adminCredentialsRepository;
        private readonly IPasswordResetRepository _passwordResetRepository;
        private readonly IResetIdentifierService _resetIdentifierService;
        private readonly ILog _log;

        public AdminCredentialsService(
            IAdminCredentialsRepository adminCredentialsRepository,
            IPasswordResetRepository passwordResetRepository,
            IResetIdentifierService resetIdentifierService,
            ILogFactory logFactory)
        {
            _adminCredentialsRepository = adminCredentialsRepository;
            _passwordResetRepository = passwordResetRepository;
            _resetIdentifierService = resetIdentifierService;
            _log = logFactory.CreateLog(this);
        }

        public async Task CreateAsync(string adminId, string login, string password)
        {
            var adminCredentials = await _adminCredentialsRepository.GetByLoginAsync(login);

            if (adminCredentials != null)
                throw new AdminCredentialsAlreadyExistsException();

            adminCredentials = new AdminCredentials {AdminId = adminId, Login = login};

            adminCredentials.SetPassword(password);

            await _adminCredentialsRepository.InsertAsync(adminCredentials);

            _log.Info("Admin credentials created.", context: $"adminId: {adminId}");
        }

        public async Task<(bool isValid, string adminId)> ValidateAsync(string login, string password)
        {
            var adminCredentials = await _adminCredentialsRepository.GetByLoginAsync(login);

            if (adminCredentials == null)
                throw new AdminCredentialsNotFoundException();

            return (adminCredentials.CheckPassword(password), adminCredentials.AdminId);
        }

        public async Task<string> GenerateResetIdentifierAsync(string adminId)
        {
            string identifier;
            TimeSpan identifierTimeSpan;

            try
            {
                (identifier, identifierTimeSpan) = await _resetIdentifierService.GenerateAsync(adminId);
            }
            catch (IdentifierRequestsExceededException)
            {
                _log.Warning("Too much admin password reset identifier requests", context: $"adminId: {adminId}");
                throw;
            }

            await _passwordResetRepository.CreateOrUpdateIdentifierAsync(adminId, identifier, identifierTimeSpan);

            _log.Info("Admin password reset identifier generated",
                context: $"adminId: {adminId}; valid until: {DateTime.UtcNow + identifierTimeSpan}");

            return identifier;
        }

        public async Task<bool> ValidateResetIdentifierAsync(string identifier)
        {
            var resetIdentifier = await _passwordResetRepository.GetByIdentifierAsync(identifier);

            if (resetIdentifier == null)
                throw new IdentifierDoesNotExistException();

            return DateTime.UtcNow < resetIdentifier.ExpiresAt;
        }

        public async Task ResetPasswordAsync(string login, string password, string identifier)
        {
            var adminCredentials = await _adminCredentialsRepository.GetByLoginAsync(login);

            if (adminCredentials == null)
                throw new AdminCredentialsNotFoundException();

            var resetIdentifier = await _passwordResetRepository.GetByIdentifierAsync(identifier);

            if (resetIdentifier == null)
                throw new IdentifierDoesNotExistException();

            if (resetIdentifier.ExpiresAt < DateTime.UtcNow)
                throw new IdentifierHasExpiredException();

            if (resetIdentifier.Identifier != identifier)
                throw new IdentifierMismatchException();

            adminCredentials.SetPassword(password);
            adminCredentials.Login = login;

            await _adminCredentialsRepository.UpdateAsync(adminCredentials);

            await _resetIdentifierService.ClearAsync(adminCredentials.AdminId);

            await _passwordResetRepository.RemoveAsync(adminCredentials.AdminId);

            _log.Info("Admin password has been reset.", context: $"adminId: {adminCredentials.AdminId}");
        }

        public async Task ChangePasswordAsync(string login, string password)
        {
            var adminCredentials = await _adminCredentialsRepository.GetByLoginAsync(login);

            if (adminCredentials == null)
                throw new AdminCredentialsNotFoundException();

            //We are setting the login again cause the one we got from the DB is hashed
            adminCredentials.Login = login;
            adminCredentials.SetPassword(password);

            await _adminCredentialsRepository.UpdateAsync(adminCredentials);

            _log.Info("Admin credentials updated.", context: $"adminId: {adminCredentials.AdminId}");
        }

        public async Task RemoveAsync(string login)
        {
            var result = await _adminCredentialsRepository.DeleteAsync(login);

            if (result)
                _log.Info("Admin credentials removed.", context: $"login: {login.SanitizeEmail()}");
        }
    }
}
