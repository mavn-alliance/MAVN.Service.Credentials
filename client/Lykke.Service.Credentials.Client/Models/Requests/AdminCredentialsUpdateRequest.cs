using JetBrains.Annotations;

namespace Lykke.Service.Credentials.Client.Models.Requests
{
    /// <summary>
    /// Represents administrator credentials update request.
    /// </summary>
    [PublicAPI]
    public class AdminCredentialsUpdateRequest : Credentials
    {
        /// <summary>
        /// The admin identifier.
        /// </summary>
        public string AdminId { get; set; }
    }
}
