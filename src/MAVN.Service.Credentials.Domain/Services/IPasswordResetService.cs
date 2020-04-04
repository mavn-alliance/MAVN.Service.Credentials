using System.Threading.Tasks;
using MAVN.Service.Credentials.Domain.Enums;
using MAVN.Service.Credentials.Domain.Models;

namespace MAVN.Service.Credentials.Domain.Services
{
    public interface IPasswordResetService
    {
        Task<PasswordResetModel> CreateOrUpdateIdentifierAsync(string customerId);

        Task<PasswordResetErrorCodes> PasswordResetAsync(string customerEmail, string identifier, string newPassword);

        Task<ValidateIdentifierErrorCodes> ValidateResetIdentifier(string identifier);
    }
}
