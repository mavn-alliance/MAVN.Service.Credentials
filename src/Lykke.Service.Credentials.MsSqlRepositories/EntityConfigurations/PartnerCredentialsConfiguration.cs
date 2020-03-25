using Lykke.Service.Credentials.MsSqlRepositories.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Lykke.Service.Credentials.MsSqlRepositories.EntityConfigurations
{
    public class PartnerCredentialsConfiguration : IEntityTypeConfiguration<PartnerCredentialsEntity>
    {
        public void Configure(EntityTypeBuilder<PartnerCredentialsEntity> builder)
        {
            builder.Property(x => x.PartnerId).HasDefaultValueSql("newid()");

            builder.HasIndex(x => x.PartnerId).IsUnique(true);
        }
    }
}
