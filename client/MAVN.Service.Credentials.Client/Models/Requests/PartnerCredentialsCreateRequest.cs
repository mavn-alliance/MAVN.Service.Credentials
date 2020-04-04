using JetBrains.Annotations;

namespace MAVN.Service.Credentials.Client.Models.Requests
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
