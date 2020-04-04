using JetBrains.Annotations;

namespace MAVN.Service.Credentials.Client.Models.Requests
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
