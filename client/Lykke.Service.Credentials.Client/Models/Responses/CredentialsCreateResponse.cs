using JetBrains.Annotations;

namespace Lykke.Service.Credentials.Client.Models.Responses
{
    /// <summary>
    /// Credentials create response.
    /// </summary>
    [PublicAPI]
    public class CredentialsCreateResponse
    {
        /// <summary>
        /// Credentials creation error.
        /// </summary>
        public CredentialsError Error { get; set; }
    }
}
