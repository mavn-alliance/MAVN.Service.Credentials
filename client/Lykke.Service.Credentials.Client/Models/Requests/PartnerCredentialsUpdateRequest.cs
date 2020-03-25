using JetBrains.Annotations;

namespace Lykke.Service.Credentials.Client.Models.Requests
{
    /// <summary>
    /// Represents partner credentials update request.
    /// </summary>
    [PublicAPI]
    public class PartnerCredentialsUpdateRequest : PartnerCredentials
    {
        /// <summary>
        /// The partner identifier.
        /// </summary>
        public string PartnerId { get; set; }
    }
}
