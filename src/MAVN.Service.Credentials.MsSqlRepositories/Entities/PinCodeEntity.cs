using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Common.PasswordTools;

namespace MAVN.Service.Credentials.MsSqlRepositories.Entities
{
    [Table("customer_pin_codes")]
    public class PinCodeEntity : IPasswordKeeping
    {
        [Key]
        [Column("customer_id")]
        public string CustomerId { get; set; }

        [NotMapped]
        public string PinCode { get; set; }

        [Required]
        [Column("salt")]
        public string Salt { get; set; }

        [Required]
        [Column("hash")]
        public string Hash { get; set; }

        public static PinCodeEntity Create(string customerId, string pinCode)
        {
            var result = new PinCodeEntity
            {
                CustomerId = customerId
            };

            result.SetPassword(pinCode);

            return result;
        }
    }
}
