using Common.PasswordTools;

namespace MAVN.Service.Credentials.Domain.Models
{
    public class AdminCredentials : IPasswordKeeping
    {
        public string AdminId { get; set; }

        public string Login { get; set; }

        public string Salt { get; set; }

        public string Hash { get; set; }
    }
}
