using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Lykke.Service.Credentials.Domain.Models;

namespace Lykke.Service.Credentials.MsSqlRepositories.Entities
{
    [Table("password_reset")]
    public class PasswordResetEntity : IResetIdentifier
    {
        [Key]
        [Required]
        [Column("customer_id")]
        public string CustomerId { get; set; }

        [Required]
        [Column("identifier")]
        public string Identifier { get; set; }

        [Required]
        [Column("created_at")]
        public DateTime CreatedAt { get; set; }
        
        [Required]
        [Column("expires_at")]
        public DateTime ExpiresAt { get; set; }

        public static PasswordResetEntity Create(
            string customerId, string identifier, TimeSpan identifierTimeSpan)
        {
            var result = new PasswordResetEntity()
            {
                CustomerId = customerId,
                Identifier = identifier,
                CreatedAt = DateTime.UtcNow,
                ExpiresAt = DateTime.UtcNow + identifierTimeSpan
            };

            return result;
        }

    }
}
