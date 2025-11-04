using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TrueCodeApp.Core.Domain.Entities;

namespace TrueCodeApp.Core.Infrastructure.Persistence.Configurations;

public class UserCurrencyConfigurations : IEntityTypeConfiguration<UserCurrency>
{
    public void Configure(EntityTypeBuilder<UserCurrency> builder)
    {
        builder.ToTable("user_currencies", "public");
        
        builder.HasKey(x => new { x.UserId, x.CurrencyId });

        builder.HasOne(gc => gc.User)
            .WithMany(g => g.UserCurrencies)
            .HasForeignKey(gc => gc.UserId);

        builder.HasOne(gc => gc.Currency)
            .WithMany(g => g.UserCurrencies)
            .HasForeignKey(gc => gc.CurrencyId);
    }
}