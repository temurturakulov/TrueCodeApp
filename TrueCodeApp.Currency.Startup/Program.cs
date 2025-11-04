using Serilog;
using TrueCodeApp.Core.Domain.Services;
using TrueCodeApp.Core.Infrastructure.Definitions;
using TrueCodeApp.Core.Startup.Definitions;
using TrueCodeApp.Core.Web.Definitions;
using TrueCodeApp.Core.Web.Services;
using TrueCodeApp.Currency.Infrastructure.Definitions;

namespace TrueCodeApp.Currency.Startup;

public class Program
{
    public static void Main(string[] args)
    {
        try
        {
            var builder = WebApplication.CreateBuilder(args);

            // 1. Настройка логирования и конфигурации
            builder.AddAppSettings();
            builder.AddSerilogLogger();

            // 2. Добавляем контроллеры
            builder.Services.AddControllers();

            // 3. Подключаем все сервисы из IAppDefinition
            builder.Services.AddAppDefinitions(builder.Configuration, typeof(IAppDefinition).Assembly);
            builder.Services.AddAppDefinitions(builder.Configuration, typeof(ServiceDefinition).Assembly);
            builder.Services.AddAppDefinitions(builder.Configuration, typeof(CurrencyServiceDefinitions).Assembly);
            builder.Services.AddScoped<IUserIdentityService, UserIdentityService>();

            // 4. Вызываем ConfigureServices у всех WebAppDefinition ДО сборки
            var webDefinitions = typeof(WebAppDefinition).Assembly
                .GetTypes()
                .Where(t => typeof(WebAppDefinition).IsAssignableFrom(t) && !t.IsAbstract)
                .Select(t => (WebAppDefinition)Activator.CreateInstance(t)!)
                .OrderBy(d => d.OrderIndex)
                .ToList();

            foreach (var def in webDefinitions)
                def.ConfigureServices(builder);

            // 5. Строим приложение
            var app = builder.Build();

            // 6. Middleware pipeline
            app.UseSerilogLogger();
            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();

            // 7. Конфигурируем WebAppDefinition.Configure(app)
            foreach (var def in webDefinitions)
                def.Configure(app);

            // 8. MapControllers
            app.MapControllers();

            // 10. Запуск
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