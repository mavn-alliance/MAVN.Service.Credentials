using Lykke.HttpClientGenerator;
using Lykke.Service.Credentials.Client.Api;

namespace Lykke.Service.Credentials.Client
{
    /// <inheritdoc />
    public class CredentialsClient : ICredentialsClient
    {
        /// <summary>
        /// Provides methods to work with partners credentials.
        /// </summary>
        public IPartnersApi Partners { get; }

        /// <summary>C-tor</summary>
        public CredentialsClient(IHttpClientGenerator httpClientGenerator)
        {
            Api = httpClientGenerator.Generate<ICredentialsApi>();
            Admins = httpClientGenerator.Generate<IAdminsApi>();
            Partners = httpClientGenerator.Generate<IPartnersApi>();
        }

        /// <inheritdoc />
        public ICredentialsApi Api { get; }

        /// <inheritdoc />
        public IAdminsApi Admins { get; }
    }
}
