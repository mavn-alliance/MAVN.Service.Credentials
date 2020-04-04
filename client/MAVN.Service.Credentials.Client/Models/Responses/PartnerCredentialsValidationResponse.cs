using JetBrains.Annotations;

namespace MAVN.Service.Credentials.Client.Models.Responses
{
    /// <summary>
    /// Represents partner credentials validation response.
    /// </summary>
    [PublicAPI]
    public class PartnerCredentialsValidationResponse
    {
        /// <summary>
        /// Validated partner id
        /// </summary>
        public string PartnerId { get; set; }

        /// <summary>
        /// The credentials validation error.
        /// </summary>
        public CredentialsError Error { get; set; }
    }
}
