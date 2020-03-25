namespace Lykke.Service.Credentials.Domain.Models
{
    public interface ICustomerCredentials
    {
        string CustomerId { get; }

        string Login { get; }

        string Password { get; }
    }
}
