using JetBrains.Annotations;

namespace MAVN.Service.Credentials.Client.Models.Requests
{
    /// <summary>
    /// Represents customer credentials update request.
    /// </summary>
    [PublicAPI]
    public class CredentialsUpdateRequest : Credentials
    {
        /// <summary>
        /// The customer identifier.
        /// </summary>
        public string CustomerId { get; set; }
    }
}
