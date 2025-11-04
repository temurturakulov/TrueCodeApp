using System.Reflection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace TrueCodeApp.Core.Infrastructure.Definitions;

public static class AppDefinitionExtensions
{
    public static void AddAppDefinitions(this IServiceCollection services, IConfiguration configuration, Assembly assembly)
    {
        var serviceProvider = services.BuildServiceProvider();

        var definitionTypes = assembly
            .GetTypes()
            .Where(t => typeof(IAppDefinition).IsAssignableFrom(t) && !t.IsInterface && !t.IsAbstract);

        foreach (var type in definitionTypes)
        {
            var definition = ActivatorUtilities.CreateInstance(serviceProvider, type) as IAppDefinition;
            definition?.ConfigureServices(services, configuration);
        }
    }
}