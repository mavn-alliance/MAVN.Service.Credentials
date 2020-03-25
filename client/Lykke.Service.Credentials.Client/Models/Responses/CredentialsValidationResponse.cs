using JetBrains.Annotations;

namespace Lykke.Service.Credentials.Client.Models.Responses
{
    /// <summary>
    /// Credentials validation response.
    /// </summary>
    [PublicAPI]
    public class CredentialsValidationResponse
    {
        /// <summary>
        /// Validated customer id
        /// </summary>
        public string CustomerId { get; set; }

        /// <summary>
        /// Credentials validation error.
        /// </summary>
        public CredentialsError Error { get; set; }
    }
}
