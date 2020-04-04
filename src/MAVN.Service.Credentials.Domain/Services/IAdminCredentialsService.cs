using System.Threading.Tasks;

namespace MAVN.Service.Credentials.Domain.Services
{
    public interface IAdminCredentialsService
    {
        Task CreateAsync(string adminId, string login, string password);

        Task<(bool isValid, string adminId)> ValidateAsync(string login, string password);

        Task<string> GenerateResetIdentifierAsync(string adminId);

        Task<bool> ValidateResetIdentifierAsync(string identifier);

        Task ResetPasswordAsync(string login, string password, string identifier);

        Task ChangePasswordAsync(string login, string password);

        Task RemoveAsync(string login);
    }
}
