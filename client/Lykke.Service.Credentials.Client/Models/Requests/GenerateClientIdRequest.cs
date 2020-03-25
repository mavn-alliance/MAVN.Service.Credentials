namespace Lykke.Service.Credentials.Client.Models.Requests
{
    /// <summary>
    /// Represents a request to generate Id of the Client
    /// </summary>
    public class GenerateClientIdRequest
    {
        /// <summary>
        /// Length of the generated Id
        /// </summary>
        public int Length { set; get; }
    }
}