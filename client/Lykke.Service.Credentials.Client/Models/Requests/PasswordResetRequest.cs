using System.ComponentModel.DataAnnotations;

namespace Lykke.Service.Credentials.Client.Models.Requests
{
    /// <summary>
    /// Contains information for the Password Reset Request
    /// </summary>
    public class PasswordResetRequest
    {
        /// <summary>
        /// Email of the Customer
        /// </summary>
        [Required]
        [EmailAddress]
        public string CustomerEmail { get; set; }
        /// <summary>
        /// Password Reset Identifier used to determinate if a Password Request was made
        /// </summary>
        [Required]
        public string ResetIdentifier { get; set; }

        /// <summary>
        /// The new Customer Password
        /// </summary>
        [Required]
        public string Password { get; set; }
    }
}
