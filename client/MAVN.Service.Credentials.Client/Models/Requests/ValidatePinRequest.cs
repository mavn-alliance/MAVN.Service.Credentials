using System.ComponentModel.DataAnnotations;

namespace MAVN.Service.Credentials.Client.Models.Requests
{
    public class ValidatePinRequest
    {
        /// <summary>
        /// Id of the customer
        /// </summary>
        [Required]
        public string CustomerId { get; set; }

        /// <summary>
        /// PIN code
        /// </summary>
        [Required]
        public string PinCode { get; set; }
    }
}
