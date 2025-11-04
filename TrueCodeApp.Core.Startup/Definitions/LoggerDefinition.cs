using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Serilog;

namespace TrueCodeApp.Core.Startup.Definitions;

public static class LoggerDefinition
{
    public static WebApplicationBuilder AddSerilogLogger(this WebApplicationBuilder builder)
    {
        const string jsonFile = "serilog.json";
        var path = AppDomain.CurrentDomain.BaseDirectory;

        builder.Configuration.AddJsonFile(Path.Combine(path, jsonFile), true, false);

        var logConfigurator = new LoggerConfiguration()
            .Enrich.WithMachineName()
            .Enrich.WithEnvironmentUserName()
            .Enrich.WithProperty("Environment", builder.Environment.EnvironmentName)
            .Enrich.WithProperty("Application", AppDomain.CurrentDomain.FriendlyName)
            .ReadFrom.Configuration(builder.Configuration);

        Log.Logger = logConfigurator.CreateLogger();
        builder.Host.UseSerilog();

        return builder;
    }

    public static WebApplication UseSerilogLogger(this WebApplication app)
    {
        app.UseSerilogRequestLogging(options => options.IncludeQueryInRequestPath = true);
        return app;
    }
}