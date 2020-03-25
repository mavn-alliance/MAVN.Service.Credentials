using JetBrains.Annotations;

namespace Lykke.Service.Credentials.Client.Models.Requests
{
    /// <summary>
    /// Represents partner credentials create request.
    /// </summary>
    [PublicAPI]
    public class PartnerCredentialsCreateRequest : PartnerCredentials
    {
        /// <summary>
        /// The partner identifier.
        /// </summary>
        public string PartnerId { get; set; }
    }
}
