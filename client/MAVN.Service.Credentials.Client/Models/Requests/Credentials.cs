namespace MAVN.Service.Credentials.Client.Models.Requests
{
    /// <summary>
    /// Represents a credentials details.
    /// </summary>
    public abstract class Credentials
    {
        /// <summary>
        /// Tha login.
        /// </summary>
        public string Login { get; set; }

        /// <summary>
        /// The password.
        /// </summary>
        public string Password { get; set; }
    }
}
