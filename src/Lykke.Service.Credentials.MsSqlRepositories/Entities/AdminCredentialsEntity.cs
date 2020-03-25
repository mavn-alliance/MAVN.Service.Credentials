using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Lykke.Service.Credentials.MsSqlRepositories.Entities
{
    [Table("admin_credentials")]
    public class AdminCredentialsEntity
    {
        [Key]
        [Required]
        [Column("login")]
        public string Login { get; set; }

        [Required]
        [Column("admin_id")]
        public string AdminId { get; set; }

        [Required]
        [Column("salt")]
        public string Salt { get; set; }

        [Required]
        [Column("hash")]
        public string Hash { get; set; }
    }
}
