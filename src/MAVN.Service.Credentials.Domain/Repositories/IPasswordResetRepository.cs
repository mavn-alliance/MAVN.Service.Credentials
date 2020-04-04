using System;
using System.Threading.Tasks;
using MAVN.Service.Credentials.Domain.Models;

namespace MAVN.Service.Credentials.Domain.Repositories
{
    public interface IPasswordResetRepository
    {
        Task CreateOrUpdateIdentifierAsync(string customerId, string identifier, TimeSpan identifierTimeSpan);

        Task<IResetIdentifier> GetIdentifierAsync(string customerId);

        Task<IResetIdentifier> GetByIdentifierAsync(string resetIdentifier);

        Task RemoveAsync(string customerId);
    }
}
