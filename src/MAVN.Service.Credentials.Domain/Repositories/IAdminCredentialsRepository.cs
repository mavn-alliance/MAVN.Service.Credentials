using System.Threading.Tasks;
using MAVN.Service.Credentials.Domain.Models;

namespace MAVN.Service.Credentials.Domain.Repositories
{
    public interface IAdminCredentialsRepository
    {
        Task<AdminCredentials> GetByLoginAsync(string login);
        
        Task InsertAsync(AdminCredentials adminCredentials);
        
        Task UpdateAsync(AdminCredentials adminCredentials);
        
        Task<bool> DeleteAsync(string login);
    }
}
