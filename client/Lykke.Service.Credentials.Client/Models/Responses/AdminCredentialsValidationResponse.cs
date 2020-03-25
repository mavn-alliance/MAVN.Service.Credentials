using JetBrains.Annotations;

namespace Lykke.Service.Credentials.Client.Models.Responses
{
    /// <summary>
    /// Represents credentials validation response.
    /// </summary>
    [PublicAPI]
    public class AdminCredentialsValidationResponse
    {
        /// <summary>
        /// The administrator identifier.
        /// </summary>
        public string AdminId { get; set; }

        /// <summary>
        /// The credentials validation error.
        /// </summary>
        public CredentialsError Error { get; set; }
    }
}
