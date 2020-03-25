using System.Threading.Tasks;
using Common.PasswordTools;
using Lykke.Service.Credentials.Domain.Models;

namespace Lykke.Service.Credentials.Domain.Repositories
{
    public interface ICustomerCredentialsRepository
    {
        Task<IHashedCustomerCredentials> GetAsync(string login);

        Task<bool> RemoveAsync(string login);

        Task CreateAsync(
            string customerId,
            string login,
            string password);

        Task UpdatePasswordAsync(string login, string password);

        Task<bool> UpdateLoginAsync(string oldLogin, string newLogin);

        Task SetPinAsync(string customerId, string pinCode);

        Task<IPasswordKeeping> GetPinByCustomerIdAsync(string customerId);

        Task<IHashedCustomerCredentials> GetByCustomerIdAsync(string customerId);
    }
}
