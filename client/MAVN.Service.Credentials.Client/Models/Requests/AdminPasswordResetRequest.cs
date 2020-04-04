using JetBrains.Annotations;

namespace MAVN.Service.Credentials.Client.Models.Requests
{
    /// <summary>
    /// Represents administrator password reset request.
    /// </summary>
    [PublicAPI]
    public class AdminPasswordResetRequest : Credentials
    {
        /// <summary>
        /// The password reset identifier.
        /// </summary>
        public string ResetIdentifier { get; set; }
    }
}
