using Microsoft.EntityFrameworkCore;
using TrueCodeApp.Core.Domain.Entities;

namespace TrueCodeApp.Core.Infrastructure.Persistence;

public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : DbContext(options)
{
    public DbSet<User> Users => Set<User>();
    public DbSet<Currency> Currencies => Set<Currency>();
    public DbSet<UserCurrency> UserCurrencies => Set<UserCurrency>();
    public DbSet<Token>  Tokens => Set<Token>();
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
         
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);
    }
}