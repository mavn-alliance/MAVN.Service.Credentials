using System;
using System.Globalization;
using System.Threading.Tasks;
using Common;
using Common.Log;
using Lykke.Common.Log;
using Lykke.Service.Credentials.Domain.Enums;
using Lykke.Service.Credentials.Domain.Helpers;
using Lykke.Service.Credentials.Domain.Models;
using Lykke.Service.Credentials.Domain.Repositories;
using Lykke.Service.Credentials.Domain.Services;
using StackExchange.Redis;

namespace Lykke.Service.Credentials.DomainServices
{
    public class PasswordResetService : IPasswordResetService
    {
        private const string CustomerKeyPattern = "{0}::passwordreset::{1}";

        private readonly string _instanceName;
        private readonly int _maxAllowedRequestsNumber;
        private readonly TimeSpan _monitoredPeriod;
        private readonly TimeSpan _identifierTimeSpan;
        private readonly int _resetIdentifierLength;
        private readonly IPasswordResetRepository _passwordResetRepository;
        private readonly ICustomerCredentialsService _customerCredentialsService;
        private readonly IBase34Util _base34Util;
        private readonly ILog _log;
        private readonly IDatabase _db;

        public PasswordResetService(IConnectionMultiplexer connectionMultiplexer,
            string instanceName,
            int maxAllowedRequestsNumber,
            TimeSpan monitoredPeriod,
            TimeSpan identifierTimeSpan,
            int resetIdentifierLength,
            IPasswordResetRepository passwordResetRepository,
            ICustomerCredentialsService customerCredentialsService,
            IBase34Util base34Util,
            ILogFactory logFactory)
        {
            _instanceName = instanceName;
            _maxAllowedRequestsNumber = maxAllowedRequestsNumber;
            _monitoredPeriod = monitoredPeriod;
            _identifierTimeSpan = identifierTimeSpan;
            _resetIdentifierLength = resetIdentifierLength;
            _passwordResetRepository = passwordResetRepository;
            _customerCredentialsService = customerCredentialsService;
            _base34Util = base34Util;
            _log = logFactory.CreateLog(this);
            _db = connectionMultiplexer.GetDatabase();
        }

        public async Task<PasswordResetModel> CreateOrUpdateIdentifierAsync(string customerId)
        {
            await RecordCallAsync(customerId);

            if (await GetCallsForPeriodAsync(customerId) > _maxAllowedRequestsNumber)
            {
                _log.Info($"Customer with Id: {customerId} made to many Password reset request and was blocked");
                return new PasswordResetModel {ErrorCode = PasswordResetErrorCodes.ReachedMaximumRequestForPeriod};
            }

            var identifier = GenerateIdentifier(customerId, _resetIdentifierLength);
            await _passwordResetRepository.CreateOrUpdateIdentifierAsync(customerId, identifier, _identifierTimeSpan);
            _log.Info(
                $"Successfully generated and updated Password Reset Identifier for Customer: {customerId} which will be valid till {DateTime.UtcNow + _identifierTimeSpan}");

            return new PasswordResetModel
            {
                ErrorCode = PasswordResetErrorCodes.None,
                Identifier = identifier
            };
        }

        public async Task<PasswordResetErrorCodes> PasswordResetAsync(string customerEmail, string identifier,
            string newPassword)
        {
            var credentials = await _customerCredentialsService.GetAsync(customerEmail);

            if (credentials == null)
            {
                _log.Info("Customer credentials do not exist", customerEmail.SanitizeEmail());
                return PasswordResetErrorCodes.CustomerDoesNotExist;
            }

            var activeIdentifier = await _passwordResetRepository.GetIdentifierAsync(credentials.CustomerId);

            if (activeIdentifier == null)
                return PasswordResetErrorCodes.ThereIsNoIdentifierForThisCustomer;

            if (activeIdentifier.ExpiresAt < DateTime.UtcNow)
                return PasswordResetErrorCodes.ProvidedIdentifierHasExpired;

            if (activeIdentifier.Identifier != identifier)
                return PasswordResetErrorCodes.IdentifierMismatch;


            var updateTask = _customerCredentialsService.UpdatePasswordAsync(customerEmail, newPassword);

            var cleanUpTask = ClearAllCallRecordsAsync(credentials.CustomerId);

            await Task.WhenAll(updateTask, cleanUpTask);

            await _passwordResetRepository.RemoveAsync(credentials.CustomerId);

            _log.Info($"Successfully reset the password for {credentials.CustomerId}");

            return PasswordResetErrorCodes.None;
        }

        public async Task<ValidateIdentifierErrorCodes> ValidateResetIdentifier(string identifier)
        {
            var resetIdentifier = await _passwordResetRepository.GetByIdentifierAsync(identifier);

            if (resetIdentifier == null)
                return ValidateIdentifierErrorCodes.IdentifierDoesNotExist;

            if (resetIdentifier.ExpiresAt < DateTime.UtcNow)
                return ValidateIdentifierErrorCodes.ProvidedIdentifierHasExpired;

            return ValidateIdentifierErrorCodes.None;
        }

        private string GenerateIdentifier(string customerId, int length)
        {
            //Note: Appending the current DateTime so the generated identifier is always different.
            var inputData = customerId + DateTime.UtcNow;
            return _base34Util.GenerateBase(inputData).Substring(0, length);
        }


        private async Task ClearAllCallRecordsAsync(string customerId)
        {
            var key = GetCustomerKeyFromPattern(customerId);

            await _db.SortedSetRemoveRangeByScoreAsync(key, double.MinValue, double.MaxValue);
        }

        private async Task RecordCallAsync(string customerId)
        {
            var key = GetCustomerKeyFromPattern(customerId);

            await _db.SortedSetAddAsync(key, DateTime.UtcNow.ToString(CultureInfo.InvariantCulture), DateTime.UtcNow.Ticks);
        }

        private async Task<int> GetCallsForPeriodAsync(string customerId)
        {
            await ClearOldCallRecordsAsync(customerId);

            var key = GetCustomerKeyFromPattern(customerId);
            var now = DateTime.UtcNow;
            var activeCallRecords = await _db.SortedSetRangeByScoreAsync(key, (now - _monitoredPeriod).Ticks,
                now.Ticks);

            return activeCallRecords.Length;
        }

        private async Task ClearOldCallRecordsAsync(string customerId)
        {
            var key = GetCustomerKeyFromPattern(customerId);
            await _db.SortedSetRemoveRangeByScoreAsync(key, double.MinValue,
                (DateTime.UtcNow - _monitoredPeriod).Ticks);
        }

        private string GetCustomerKeyFromPattern(string customerId)
        {
            return string.Format(CustomerKeyPattern, _instanceName, customerId);
        }
    }
}
