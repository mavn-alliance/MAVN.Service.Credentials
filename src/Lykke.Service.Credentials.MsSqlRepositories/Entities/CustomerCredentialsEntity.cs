using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Common.PasswordTools;
using JetBrains.Annotations;
using Lykke.Service.Credentials.Domain.Models;

namespace Lykke.Service.Credentials.MsSqlRepositories.Entities
{
    [Table("customer_credentials")]
    public class CustomerCredentialsEntity : IHashedCustomerCredentials
    {
        [Key]
        [Required]
        [Column("login")]
        public string Login { get; set; }

        [Required]
        [Column("customer_id")]
        public string CustomerId { get; set; }

        [CanBeNull]
        public PinCodeEntity PinCode { get; set; }

        [NotMapped]
        public string Password { get; set; }

        [Required]
        [Column("salt")]
        public string Salt { get; set; }

        [Required]
        [Column("hash")]
        public string Hash { get; set; }

        public static CustomerCredentialsEntity Create(
            string customerId,
            string login,
            string password)
        {
            var result = new CustomerCredentialsEntity
            {
                Login = login,
                CustomerId = customerId
            };

            result.SetPassword(password);

            return result;
        }

        public CustomerCredentialsEntity Copy()
        {
            return new CustomerCredentialsEntity
            {
                Login = Login,
                CustomerId = CustomerId,
                Password = Password,
                Salt = Salt,
                Hash = Hash,
            };
        }
    }

}
