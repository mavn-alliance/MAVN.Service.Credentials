using System.Threading.Tasks;
using Lykke.Service.Credentials.Domain.Models;

namespace Lykke.Service.Credentials.Domain.Repositories
{
    public interface IAdminCredentialsRepository
    {
        Task<AdminCredentials> GetByLoginAsync(string login);
        
        Task InsertAsync(AdminCredentials adminCredentials);
        
        Task UpdateAsync(AdminCredentials adminCredentials);
        
        Task<bool> DeleteAsync(string login);
    }
}
