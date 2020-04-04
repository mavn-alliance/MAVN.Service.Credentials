using MAVN.Service.Credentials.Client.Enums;

namespace MAVN.Service.Credentials.Client.Models.Responses
{
    public class ValidatePinResponse
    {
        /// <summary>
        /// Error code
        /// </summary>
        public PinCodeErrorCodes Error { get; set; }
    }
}
