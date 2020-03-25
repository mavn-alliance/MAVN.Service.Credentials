using System.Threading.Tasks;
using Lykke.Service.Credentials.Domain.Enums;
using Lykke.Service.Credentials.Domain.Models;

namespace Lykke.Service.Credentials.Domain.Services
{
    public interface IPasswordResetService
    {
        Task<PasswordResetModel> CreateOrUpdateIdentifierAsync(string customerId);

        Task<PasswordResetErrorCodes> PasswordResetAsync(string customerEmail, string identifier, string newPassword);

        Task<ValidateIdentifierErrorCodes> ValidateResetIdentifier(string identifier);
    }
}
