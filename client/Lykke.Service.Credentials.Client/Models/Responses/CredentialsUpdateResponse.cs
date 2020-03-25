using JetBrains.Annotations;

namespace Lykke.Service.Credentials.Client.Models.Responses
{
    /// <summary>
    /// Credentials update response.
    /// </summary>
    [PublicAPI]
    public class CredentialsUpdateResponse
    {
        /// <summary>
        /// Credentials update error.
        /// </summary>
        public CredentialsError Error { get; set; }
    }
}
