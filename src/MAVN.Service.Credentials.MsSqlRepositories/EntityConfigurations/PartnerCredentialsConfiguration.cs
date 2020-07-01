using MAVN.Service.Credentials.MsSqlRepositories.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MAVN.Service.Credentials.MsSqlRepositories.EntityConfigurations
{
    public class PartnerCredentialsConfiguration : IEntityTypeConfiguration<PartnerCredentialsEntity>
    {
        public void Configure(EntityTypeBuilder<PartnerCredentialsEntity> builder)
        {
            builder.HasIndex(x => x.PartnerId).IsUnique(true);
        }
    }
}
