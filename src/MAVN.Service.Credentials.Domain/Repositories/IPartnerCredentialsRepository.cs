using System.Threading.Tasks;
using MAVN.Service.Credentials.Domain.Models;

namespace MAVN.Service.Credentials.Domain.Repositories
{
    public interface IPartnerCredentialsRepository
    {
        Task<PartnerCredentials> GetByClientIdAsync(string clientId);

        Task<PartnerCredentials> GetByPartnerIdAsync(string partnerId);

        Task InsertAsync(PartnerCredentials partnerCredentials);

        Task<bool> DeleteAsync(string clientId);

        Task UpdateAsync(PartnerCredentials partnerCredentials);
    }
}
