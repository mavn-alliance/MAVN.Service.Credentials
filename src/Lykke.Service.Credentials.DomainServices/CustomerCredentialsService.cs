using System.Linq;
using System.Threading.Tasks;
using Common;
using Common.Log;
using Common.PasswordTools;
using Lykke.Common.Log;
using Lykke.Service.Credentials.Domain.Enums;
using Lykke.Service.Credentials.Domain.Models;
using Lykke.Service.Credentials.Domain.Repositories;
using Lykke.Service.Credentials.Domain.Services;

namespace Lykke.Service.Credentials.DomainServices
{
    public class CustomerCredentialsService : ICustomerCredentialsService
    {
        private readonly ICustomerCredentialsRepository _customerCredentialsRepository;
        private readonly ISettingsService _settingsService;
        private readonly ILog _log;

        public CustomerCredentialsService(ICustomerCredentialsRepository customerCredentialsRepository,
            ISettingsService settingsService,
            ILogFactory logFactory)
        {
            _customerCredentialsRepository = customerCredentialsRepository;
            _settingsService = settingsService;
            _log = logFactory.CreateLog(this);
        }

        public async Task<IHashedCustomerCredentials> GetAsync(string login)
        {
            var credentials = await _customerCredentialsRepository.GetAsync(login);
            return credentials;
        }

        public bool Validate(IHashedCustomerCredentials credentials, string password)
        {
            if (credentials.CheckPassword(password))
                return true;

            _log.Info("Password is incorrect", credentials.Login.SanitizeEmail());

            return false;
        }

        public async Task CreateAsync(
            string customerId,
            string login,
            string password)
        {
            await _customerCredentialsRepository.CreateAsync(
                customerId,
                login,
                password);

            _log.Info("New credentials created", customerId);
        }

        public async Task UpdatePasswordAsync(string login, string password)
        {
            await _customerCredentialsRepository.UpdatePasswordAsync(login, password);

            _log.Info("Customer credentials updated", login.SanitizeEmail());
        }

        public async Task<bool> UpdateLoginAsync(string oldLogin, string newLogin)
        {
            var updated = await _customerCredentialsRepository.UpdateLoginAsync(oldLogin, newLogin);

            _log.Info(
                updated
                    ? "Customer credentials are updated"
                    : "Customer credentials are not updated",
                new { oldLogin = oldLogin.SanitizeEmail(), newLogin = newLogin.SanitizeEmail() });

            return updated;
        }

        public async Task RemoveAsync(string login)
        {
            if (await _customerCredentialsRepository.RemoveAsync(login))
                _log.Info("Customer credentials deleted", login.SanitizeEmail());
        }

        public async Task<PinCodeErrorCodes> SetPinAsync(string customerId, string pinCode)
        {
            if (!IsPinValid(pinCode))
                return PinCodeErrorCodes.InvalidPin;

            var existingCustomer = await _customerCredentialsRepository.GetByCustomerIdAsync(customerId);

            if (existingCustomer == null)
                return PinCodeErrorCodes.CustomerDoesNotExist;

            var existingPin = await _customerCredentialsRepository.GetPinByCustomerIdAsync(customerId);

            if (existingPin != null)
                return PinCodeErrorCodes.PinAlreadySet;

            await _customerCredentialsRepository.SetPinAsync(customerId, pinCode);

            return PinCodeErrorCodes.None;
        }

        public async Task<PinCodeErrorCodes> UpdatePinAsync(string customerId, string pinCode)
        {
            if (!IsPinValid(pinCode))
                return PinCodeErrorCodes.InvalidPin;

            var existingCustomer = await _customerCredentialsRepository.GetByCustomerIdAsync(customerId);

            if (existingCustomer == null)
                return PinCodeErrorCodes.CustomerDoesNotExist;

            var existingPin = await _customerCredentialsRepository.GetPinByCustomerIdAsync(customerId);

            if (existingPin == null)
                return PinCodeErrorCodes.PinIsNotSet;

            await _customerCredentialsRepository.SetPinAsync(customerId, pinCode);

            return PinCodeErrorCodes.None;
        }

        public async Task<PinCodeErrorCodes> ValidatePinAsync(string customerId, string pinCode)
        {
            var pin = await _customerCredentialsRepository.GetPinByCustomerIdAsync(customerId);

            if (pin == null)
                return PinCodeErrorCodes.PinIsNotSet;

            if (!pin.CheckPassword(pinCode))
                return PinCodeErrorCodes.PinCodeMismatch;

            return PinCodeErrorCodes.None;
        }

        public async Task<bool> IsPinSetAsync(string customerId)
        {
            var pin = await _customerCredentialsRepository.GetPinByCustomerIdAsync(customerId);

            return pin != null;
        }

        private bool IsPinValid(string pinCode)
        {
            if (pinCode.Length != _settingsService.PinCodeLength)
                return false;

            var allDigits = pinCode.All(char.IsDigit);

            if (!allDigits)
                return false;

            return true;
        }
    }
}
