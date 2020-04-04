namespace MAVN.Service.Credentials.Client.Enums
{
    /// <summary>
    /// Holds information about the Errors involving the Password Reset procedure
    /// </summary>
    public enum PasswordResetError
    {
        /// <summary>
        /// There is no Error with the Generation of Reset Identifier
        /// </summary>
        None,
        /// <summary>
        /// There given Customer doesn't have identifier
        /// </summary>
        ThereIsNoIdentifierForThisCustomer,
        /// <summary>
        /// Reaching the Maximum amount of Calls per Given Period and Blocking further such actions for the same Period.
        /// </summary>
        ReachedMaximumRequestForPeriod,
        /// <summary>
        /// The Provided identifier doesn't much the Customer's one
        /// </summary>
        IdentifierMismatch,
        /// <summary>
        /// The Provided identifier has expired
        /// </summary>
        ProvidedIdentifierHasExpired,
        /// <summary>
        /// The Customer Doesn't exist
        /// </summary>
        CustomerDoesNotExist,
        
        /// <summary>
        /// Indicates that the login doesn't exist.
        /// </summary>
        LoginDoesNotExist,
        
        /// <summary>
        /// Indicates that the reset password identifier doesn't exist.
        /// </summary>
        IdentifierDoesNotExist
    }
}
