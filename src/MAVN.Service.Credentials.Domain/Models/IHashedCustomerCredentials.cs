using Common.PasswordTools;

namespace MAVN.Service.Credentials.Domain.Models
{
    public interface IHashedCustomerCredentials : ICustomerCredentials, IPasswordKeeping
    {
    }
}
