using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Npgsql;
using TrueCodeApp.Core.Infrastructure.Persistence;

namespace TrueCodeApp.Core.Infrastructure.Definitions;

public class PersistenceDefinition : IAppDefinition
{
    public void ConfigureServices(IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString(nameof(ApplicationDbContext));
        ArgumentException.ThrowIfNullOrWhiteSpace(connectionString);

        var dataSource = new NpgsqlDataSourceBuilder(connectionString)
            .Build();

        services.AddDbContext<ApplicationDbContext>(options => { options.UseNpgsql(dataSource); });
    }
}