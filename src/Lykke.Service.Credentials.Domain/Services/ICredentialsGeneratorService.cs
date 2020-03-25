namespace Lykke.Service.Credentials.Domain.Services
{
    public interface ICredentialsGeneratorService
    {
        string GenerateClientId(int length);

        string GeneratePassword(int length);
    }
}