using System.Threading.Tasks;

namespace MAVN.Service.Credentials.Domain.Services
{
    public interface IPartnerCredentialsService
    {
        Task CreateAsync(string clientId, string clientSecret, string partnerId);
        Task RemoveAsync(string clientId);
        Task UpdateAsync(string clientId, string clientSecret, string partnerId);
        Task<(bool isValid, string partnerId)> ValidateAsync(string clientId, string clientSecret);
    }
}
