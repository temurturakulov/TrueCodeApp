using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TrueCodeApp.Core.Domain.Entities;

namespace TrueCodeApp.Core.Infrastructure.Persistence.Configurations;

public class CurrencyConfigurations : IEntityTypeConfiguration<Currency>
{
    public void Configure(EntityTypeBuilder<Currency> builder)
    {
        builder.ToTable("currencies", "public");

        builder.Property(x => x.Rate).IsRequired();
        builder.Property(x => x.Name).IsRequired();
    }
}