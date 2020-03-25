using System.Data.Common;
using JetBrains.Annotations;
using Lykke.Common.MsSql;
using Lykke.Service.Credentials.MsSqlRepositories.Entities;
using Lykke.Service.Credentials.MsSqlRepositories.EntityConfigurations;
using Microsoft.EntityFrameworkCore;

namespace Lykke.Service.Credentials.MsSqlRepositories.Contexts
{
    public class CredentialsContext : MsSqlContext
    {
        private const string Schema = "credentials";

        internal DbSet<CustomerCredentialsEntity> CustomerCredentials { get; set; }
        internal DbSet<AdminCredentialsEntity> AdminCredentials { get; set; }
        internal DbSet<PasswordResetEntity> PasswordReset { get; set; }
        internal DbSet<PartnerCredentialsEntity> PartnerCredentials { get; set; }

        internal DbSet<PinCodeEntity> CustomerPinCodes { get; set; }


        // C-tor for EF migrations
        [UsedImplicitly]
        public CredentialsContext() : base(Schema)
        {
        }

        public CredentialsContext(string connectionString, bool isTraceEnabled)
            : base(Schema, connectionString, isTraceEnabled)
        {
        }

        public CredentialsContext(DbConnection dbConnection)
            : base(Schema, dbConnection)
        {
        }

        protected override void OnLykkeModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfiguration(new PartnerCredentialsConfiguration());

            modelBuilder.Entity<CustomerCredentialsEntity>()
                .HasOne(c => c.PinCode)
                .WithOne()
                .HasForeignKey<PinCodeEntity>(p => p.CustomerId)
                .HasPrincipalKey<CustomerCredentialsEntity>(c => c.CustomerId);
        }
    }
}
