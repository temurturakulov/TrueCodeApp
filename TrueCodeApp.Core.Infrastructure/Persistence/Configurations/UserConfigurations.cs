using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TrueCodeApp.Core.Domain.Entities;

namespace TrueCodeApp.Core.Infrastructure.Persistence.Configurations;

public class UserConfigurations : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.ToTable("users", "public");

        builder.Property(x => x.Login).HasMaxLength(20).IsRequired();
        builder.Property(x => x.Password).IsRequired();
        
        builder.HasIndex(x => x.Login)
            .IsUnique();
    }
}