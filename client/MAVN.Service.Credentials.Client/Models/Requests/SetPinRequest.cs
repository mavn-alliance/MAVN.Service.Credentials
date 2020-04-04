using System.ComponentModel.DataAnnotations;

namespace MAVN.Service.Credentials.Client.Models.Requests
{
    public class SetPinRequest
    {
        /// <summary>
        /// Id of the customer
        /// </summary>
        [Required]
        public string CustomerId { get; set; }
        /// <summary>
        /// The pin code
        /// </summary>
        [Required]
        public string PinCode { get; set; }
    }
}
