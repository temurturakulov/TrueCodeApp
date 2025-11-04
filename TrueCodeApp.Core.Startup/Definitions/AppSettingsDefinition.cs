using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;

namespace TrueCodeApp.Core.Startup.Definitions;

public static class AppSettingsDefinition
{
    /// <summary>
    /// Adds the application settings from the specified JSON configuration file.
    /// </summary>
    /// <param name="builder">The <see cref="WebApplicationBuilder"/> instance.</param>
    public static void AddAppSettings(this WebApplicationBuilder builder)
    {
        builder.Configuration
            .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
            .AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json", false, false)
            .AddUserSecrets(typeof(AppSettingsDefinition).Assembly, optional: true)
            .AddEnvironmentVariables();
    }
}