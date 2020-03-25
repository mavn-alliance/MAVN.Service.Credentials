namespace Lykke.Service.Credentials.Client.Models.Requests
{
    /// <summary>
    /// Login update request model.
    /// </summary>
    public class LoginUpdateRequest
    {
        /// <summary>Old login</summary>
        public string OldLogin { get; set; }

        /// <summary>New login</summary>
        public string NewLogin { get; set; }
    }
}
