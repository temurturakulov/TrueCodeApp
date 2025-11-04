using System.Text;
using Serilog;
using TrueCodeApp.Core.Domain.Services;
using TrueCodeApp.Core.Infrastructure.Definitions;
using TrueCodeApp.Core.Startup.Definitions;
using TrueCodeApp.Core.Web.Definitions;
using TrueCodeApp.Core.Web.Services;
using TrueCodeApp.JobManager.Web;
using TrueCodeApp.JobManager.Web.Services;

namespace TrueCodeApp.JobManager.Startup;

public class Program
{
    public static void Main(string[] args)
    {
        try
        {
            var builder = WebApplication.CreateBuilder(args);
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
            // 1. Настройка логирования и конфигурации
            builder.AddAppSettings();
            builder.AddSerilogLogger();
            
            builder.Services.AddAppDefinitions(builder.Configuration, typeof(IAppDefinition).Assembly);
            builder.Services.AddAppDefinitions(builder.Configuration, typeof(ServiceDefinition).Assembly);
             
            builder.Services.AddHostedService<CurrencyUpdaterService>();
             
            var app = builder.Build();

            app.UseSerilogLogger();
            
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