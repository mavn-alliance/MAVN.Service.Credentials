using System;
using System.Globalization;
using System.Threading.Tasks;
using MAVN.Service.Credentials.Domain.Exceptions;
using MAVN.Service.Credentials.Domain.Helpers;
using MAVN.Service.Credentials.Domain.Services;
using StackExchange.Redis;

namespace MAVN.Service.Credentials.DomainServices
{
    public class ResetIdentifierService : IResetIdentifierService
    {
        private const string KeyFormat = "{0}::ResetIdentifier::{1}";

        private readonly string _instanceName;
        private readonly int _maxAllowedRequestsNumber;
        private readonly TimeSpan _monitoredPeriod;
        private readonly TimeSpan _identifierTimeSpan;
        private readonly int _resetIdentifierLength;
        private readonly IBase34Util _base34Util;
        private readonly IDatabase _db;

        public ResetIdentifierService(
            IConnectionMultiplexer connectionMultiplexer,
            string instanceName,
            int maxAllowedRequestsNumber,
            TimeSpan monitoredPeriod,
            TimeSpan identifierTimeSpan,
            int resetIdentifierLength,
            IBase34Util base34Util)
        {
            _instanceName = instanceName;
            _maxAllowedRequestsNumber = maxAllowedRequestsNumber;
            _monitoredPeriod = monitoredPeriod;
            _identifierTimeSpan = identifierTimeSpan;
            _resetIdentifierLength = resetIdentifierLength;
            _base34Util = base34Util;
            _db = connectionMultiplexer.GetDatabase();
        }

        public async Task<(string identifier, TimeSpan identifierTimeSpan)> GenerateAsync(string key)
        {
            await RecordCallAsync(key);

            if (await GetCallsForPeriodAsync(key) > _maxAllowedRequestsNumber)
                throw new IdentifierRequestsExceededException();

            var identifier = GenerateIdentifier(key, _resetIdentifierLength);

            return (identifier, _identifierTimeSpan);
        }

        public async Task ClearAsync(string key)
        {
            var dbKey = GetCustomerKeyFromPattern(key);
            await _db.SortedSetRemoveRangeByScoreAsync(dbKey, double.MinValue, double.MaxValue);
        }

        private string GenerateIdentifier(string key, int length)
        {
            //Note: Appending the current DateTime so the generated identifier is always different.
            var inputData = key + DateTime.UtcNow;
            return _base34Util.GenerateBase(inputData).Substring(0, length);
        }

        private async Task RecordCallAsync(string customerId)
        {
            var key = GetCustomerKeyFromPattern(customerId);
            await _db.SortedSetAddAsync(key, DateTime.UtcNow.ToString(CultureInfo.InvariantCulture),
                DateTime.UtcNow.Ticks);
        }

        private async Task<int> GetCallsForPeriodAsync(string key)
        {
            await ClearOldCallRecordsAsync(key);

            var dbKey = GetCustomerKeyFromPattern(key);
            var now = DateTime.UtcNow;
            var activeCallRecords = await _db.SortedSetRangeByScoreAsync(dbKey, (now - _monitoredPeriod).Ticks,
                now.Ticks);

            return activeCallRecords.Length;
        }

        private async Task ClearOldCallRecordsAsync(string key)
        {
            var dbKey = GetCustomerKeyFromPattern(key);
            await _db.SortedSetRemoveRangeByScoreAsync(dbKey, double.MinValue,
                (DateTime.UtcNow - _monitoredPeriod).Ticks);
        }

        private string GetCustomerKeyFromPattern(string key)
            => string.Format(KeyFormat, _instanceName, key);
    }
}
