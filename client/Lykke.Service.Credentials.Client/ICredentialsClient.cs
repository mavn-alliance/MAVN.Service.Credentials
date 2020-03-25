using JetBrains.Annotations;
using Lykke.Service.Credentials.Client.Api;

namespace Lykke.Service.Credentials.Client
{
    /// <summary>
    /// Credentials API service client.
    /// </summary>
    [PublicAPI]
    public interface ICredentialsClient
    {
        /// <summary>
        /// Customer credentials API.
        /// </summary>
        ICredentialsApi Api { get; }

        /// <summary>
        /// Administrator credentials API.
        /// </summary>
        IAdminsApi Admins { get; }
        
        /// <summary>
        /// Provides methods to work with partners credentials.
        /// </summary>
        IPartnersApi Partners { get; }
    }
}
