using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using TrueCodeApp.Core.Infrastructure.Definitions;
using TrueCodeApp.Currency.Domain.Repositories;
using TrueCodeApp.Currency.Infrastructure.Repositories;

namespace TrueCodeApp.Currency.Infrastructure.Definitions;

public class CurrencyServiceDefinitions : IAppDefinition
{
    public void ConfigureServices(IServiceCollection services, IConfiguration configuration)
    {
        services.AddScoped<ICurrencyRepository, CurrencyRepository>();
    }
}