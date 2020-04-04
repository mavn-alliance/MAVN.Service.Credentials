using System.ComponentModel.DataAnnotations;
using JetBrains.Annotations;

namespace MAVN.Service.Credentials.Client.Models.Requests
{
    /// <summary>
    /// Holds password reset identifier for validation purposes.
    /// </summary>
    [PublicAPI]
    public class ResetIdentifierValidationRequest
    {
        /// <summary>
        /// The password reset identifier.
        /// </summary>
        [Required]
        public string ResetIdentifier { get; set; }
    }
}
