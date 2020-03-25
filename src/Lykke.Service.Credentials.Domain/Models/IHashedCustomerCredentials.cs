using Common.PasswordTools;

namespace Lykke.Service.Credentials.Domain.Models
{
    public interface IHashedCustomerCredentials : ICustomerCredentials, IPasswordKeeping
    {
    }
}
