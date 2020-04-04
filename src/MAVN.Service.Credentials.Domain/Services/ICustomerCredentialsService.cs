using System.Threading.Tasks;
using MAVN.Service.Credentials.Domain.Enums;
using MAVN.Service.Credentials.Domain.Models;

namespace MAVN.Service.Credentials.Domain.Services
{
    public interface ICustomerCredentialsService
    {
        Task<IHashedCustomerCredentials> GetAsync(string login);

        bool Validate(IHashedCustomerCredentials credentials, string password);

        Task CreateAsync(
            string customerId,
            string login,
            string password);

        Task UpdatePasswordAsync(string login, string password);

        Task<bool> UpdateLoginAsync(string oldLogin, string newLogin);

        Task RemoveAsync(string login);

        Task<PinCodeErrorCodes> SetPinAsync(string customerId, string pinCode);

        Task<PinCodeErrorCodes> ValidatePinAsync(string customerId, string pinCode);

        Task<bool> IsPinSetAsync(string customerId);

        Task<PinCodeErrorCodes> UpdatePinAsync(string customerId, string pinCode);
    }
}
