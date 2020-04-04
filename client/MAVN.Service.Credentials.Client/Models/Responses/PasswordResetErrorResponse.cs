using MAVN.Service.Credentials.Client.Enums;

namespace MAVN.Service.Credentials.Client.Models.Responses
{
    /// <summary>
    /// Response returned containing Error code in case of error
    /// </summary>
    public class PasswordResetErrorResponse
    {
        /// <summary>
        /// Holds information for Errors that may have happened during the Password Reset procedure
        /// </summary>
        public PasswordResetError Error { get; set; }
    }
}
