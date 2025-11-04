using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TrueCodeApp.Core.Domain.Entities;

namespace TrueCodeApp.Core.Infrastructure.Persistence.Configurations;

public class TokenConfigurations : IEntityTypeConfiguration<Token>
{
    public void Configure(EntityTypeBuilder<Token> builder)
    {
        builder.ToTable("tokens", "public");
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Value).IsRequired();
        builder.Property(x => x.ExpiresAt).IsRequired();

        builder.HasOne(gc => gc.User)
            .WithMany(g => g.Tokens)
            .HasForeignKey(gc => gc.UserId);
    }
}