using System;
using System.Threading.Tasks;

namespace MAVN.Service.Credentials.Domain.Services
{
    public interface IResetIdentifierService
    {
        Task<(string identifier, TimeSpan identifierTimeSpan)> GenerateAsync(string key);

        Task ClearAsync(string key);
    }
}
