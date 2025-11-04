using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace TrueCodeApp.Core.Infrastructure.Definitions;

public interface IAppDefinition
{
    void ConfigureServices(IServiceCollection services, IConfiguration configuration);
}