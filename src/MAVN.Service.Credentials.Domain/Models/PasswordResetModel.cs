using MAVN.Service.Credentials.Domain.Enums;

namespace MAVN.Service.Credentials.Domain.Models
{
    public class PasswordResetModel
    {
        public string Identifier { get; set; }
        public PasswordResetErrorCodes ErrorCode { get; set; }
    }
}
