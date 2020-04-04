namespace MAVN.Service.Credentials.Client.Enums
{
    public enum PinCodeErrorCodes
    {
        /// <summary>
        /// No error
        /// </summary>
        None,
        /// <summary>
        /// The customer does not exist
        /// </summary>
        CustomerDoesNotExist,
        /// <summary>
        /// The provided pin is not valid
        /// </summary>
        InvalidPin,
        /// <summary>
        /// Pin is already set for this customer
        /// </summary>
        PinAlreadySet,
        /// <summary>
        /// Pin is not set for customer
        /// </summary>
        PinIsNotSet,
        /// <summary>
        /// Pin code mismatch
        /// </summary>
        PinCodeMismatch,
    }
}
