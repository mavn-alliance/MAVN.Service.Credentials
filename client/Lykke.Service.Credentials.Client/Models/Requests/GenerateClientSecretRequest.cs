namespace Lykke.Service.Credentials.Client.Models.Requests
{
    /// <summary>
    /// Represents a request to generate Secret for the Client
    /// </summary>
    public class GenerateClientSecretRequest
    {
        /// <summary>
        /// Length of the generated Secret
        /// </summary>
        public int Length { set; get; }
    }
}