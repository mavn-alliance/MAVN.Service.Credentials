using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MAVN.Service.Credentials.MsSqlRepositories.Entities
{
    [Table("partner_credentials")]
    public class PartnerCredentialsEntity
    {
        [Key]
        [Required]
        [Column("partner_id")]
        public string PartnerId { get; set; }

        [Required]
        [Column("client_id")]
        public string ClientId { get; set; }

        [Required]
        [Column("salt")]
        public string Salt { get; set; }

        [Required]
        [Column("hash")]
        public string Hash { get; set; }
    }
}
