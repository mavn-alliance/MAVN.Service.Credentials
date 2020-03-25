using Common.PasswordTools;

namespace Lykke.Service.Credentials.Domain.Models
{
    public class PartnerCredentials : IPasswordKeeping
    {
        public string ClientId { get; set; }

        public string Salt { get; set; }

        public string Hash { get; set; }

        public string PartnerId { get; set; }
    }
}
