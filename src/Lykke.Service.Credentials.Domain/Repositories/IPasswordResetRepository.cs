using System;
using System.Threading.Tasks;
using Lykke.Service.Credentials.Domain.Models;

namespace Lykke.Service.Credentials.Domain.Repositories
{
    public interface IPasswordResetRepository
    {
        Task CreateOrUpdateIdentifierAsync(string customerId, string identifier, TimeSpan identifierTimeSpan);

        Task<IResetIdentifier> GetIdentifierAsync(string customerId);

        Task<IResetIdentifier> GetByIdentifierAsync(string resetIdentifier);

        Task RemoveAsync(string customerId);
    }
}
