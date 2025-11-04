using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using TrueCodeApp.Core.Domain.Services;
using TrueCodeApp.Core.Infrastructure.Configurations;
using TrueCodeApp.Core.Infrastructure.Services;

namespace TrueCodeApp.Core.Infrastructure.Definitions;

public class ServiceDefinitions : IAppDefinition
{
    public void ConfigureServices(IServiceCollection services, IConfiguration configuration)
    {
        services.AddScoped<IJwtTokenService, JwtTokenService>();

        services.Configure<TokenConfigurations>(
            configuration.GetSection("TokenConfigurations"));

        services.AddSingleton(TimeProvider.System);
        
        services.AddScoped<ITokenRevocationService, TokenRevocationService>();
    }
}