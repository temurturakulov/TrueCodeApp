using Serilog;
using TrueCodeApp.Core.Infrastructure.Definitions;
using TrueCodeApp.Core.Startup.Definitions;
using TrueCodeApp.Core.Web.Definitions;
using TrueCodeApp.User.Infrastructure.Definitions;

namespace TrueCodeApp.User.Startup;

public class Program
{
    public static void Main(string[] args)
    {
        try
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.AddAppSettings();
            builder.AddSerilogLogger();
            builder.Services.AddControllers();
            builder.Services.AddAppDefinitions(builder.Configuration, typeof(IAppDefinition).Assembly);
            builder.Services.AddAppDefinitions(builder.Configuration, typeof(ServiceDefinition).Assembly);
            builder.Services.AddAppDefinitions(builder.Configuration, typeof(UserServiceDefinitions).Assembly);

            var webDefinitions = typeof(WebAppDefinition).Assembly
                .GetTypes()
                .Where(t => typeof(WebAppDefinition).IsAssignableFrom(t) && !t.IsAbstract)
                .Select(t => (WebAppDefinition)Activator.CreateInstance(t)!)
                .OrderBy(d => d.OrderIndex)
                .ToList();

            foreach (var def in webDefinitions)
                def.ConfigureServices(builder);

            var app = builder.Build();

            app.UseSerilogLogger();
            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();

            foreach (var def in webDefinitions)
                def.Configure(app);

            app.MapControllers();
            app.Run();
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            Log.Fatal(e, "The application failed to start");
        }
        finally
        {
            Log.CloseAndFlush();
        }
    }
}