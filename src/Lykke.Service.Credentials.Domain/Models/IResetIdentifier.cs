using System;

namespace Lykke.Service.Credentials.Domain.Models
{
    public interface IResetIdentifier
    {
         string CustomerId { get; set; }
         string Identifier { get; set; }
         DateTime CreatedAt { get; set; }
         DateTime ExpiresAt { get; set; }
    }
}
