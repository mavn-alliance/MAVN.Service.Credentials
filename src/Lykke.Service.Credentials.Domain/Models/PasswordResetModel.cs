using Lykke.Service.Credentials.Domain.Enums;

namespace Lykke.Service.Credentials.Domain.Models
{
    public class PasswordResetModel
    {
        public string Identifier { get; set; }
        public PasswordResetErrorCodes ErrorCode { get; set; }
    }
}
