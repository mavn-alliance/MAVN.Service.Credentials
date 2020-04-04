using JetBrains.Annotations;

namespace MAVN.Service.Credentials.Client.Models.Requests
{
    /// <summary>
    /// Represents administrator credentials create request.
    /// </summary>
    [PublicAPI]
    public class AdminCredentialsCreateRequest : Credentials
    {
        /// <summary>
        /// The admin identifier.
        /// </summary>
        public string AdminId { get; set; }
    }
}
