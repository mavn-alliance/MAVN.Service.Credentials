namespace Lykke.Service.Credentials.Client.Models.Requests
{
    /// <summary>
    /// Represents partner credentials details.
    /// </summary>
    public abstract class PartnerCredentials
    {
        /// <summary>
        /// Partner client id.
        /// </summary>
        public string ClientId { get; set; }

        /// <summary>
        /// Partner client secret.
        /// </summary>
        public string ClientSecret { get; set; }
    }
}
