using MAVN.Service.Credentials.Client.Enums;

namespace MAVN.Service.Credentials.Client.Models.Responses
{
    /// <summary>
    /// Class which holds response for validation of identifier
    /// </summary>
    public class ResetIdentifierValidationResponse
    {
        /// <summary>
        /// Holds information about business errors
        /// </summary>
        public ValidateIdentifierError Error { get; set; }
    }
}
