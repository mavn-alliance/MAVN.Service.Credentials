using System;

namespace MAVN.Service.Credentials.Domain.Models
{
    public class ResetIdentifier : IResetIdentifier
    {
        public ResetIdentifier()
        {
        }

        public ResetIdentifier(string customerId, string identifier, DateTime createdAt, DateTime expiresAt)
        {
            CustomerId = customerId;
            Identifier = identifier;
            CreatedAt = createdAt;
            ExpiresAt = expiresAt;
        }

        public string CustomerId { get; set; }

        public string Identifier { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime ExpiresAt { get; set; }
    }
}
