using System.Globalization;
using System.Text;
using System.Xml.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using TrueCodeApp.Core.Domain.Entities;
using TrueCodeApp.Core.Infrastructure.Persistence;

namespace TrueCodeApp.JobManager.Web.Services;

public class CurrencyUpdaterService : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<CurrencyUpdaterService> _logger;

    public CurrencyUpdaterService(IServiceProvider serviceProvider, ILogger<CurrencyUpdaterService> logger)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
    }
    
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("CurrencyUpdaterService started.");
        using var scope = _serviceProvider.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                await UpdateCurrencies(dbContext);
                _logger.LogInformation("Currencies updated successfully.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating currencies.");
            }

            _logger.LogInformation("Job next start in 24 hours.");
            // Пауза 24 часа
            await Task.Delay(TimeSpan.FromHours(24), stoppingToken);
        }
    }

    private async Task UpdateCurrencies(ApplicationDbContext dbContext)
    {
        var xmlUrl = "http://www.cbr.ru/scripts/XML_daily.asp";
        using var client = new HttpClient();
        var bytes = await client.GetByteArrayAsync(xmlUrl);
        var xml = Encoding.GetEncoding("windows-1251").GetString(bytes);
         
        var doc = XDocument.Parse(xml);

        foreach (var elem in doc.Descendants("Valute"))
        {
            var charCode = elem.Element("CharCode")?.Value;
            var valueStr = elem.Element("Value")?.Value;

            if (charCode != null && decimal.TryParse(valueStr?.Replace(",", "."), NumberStyles.Any,
                    CultureInfo.InvariantCulture, out var rate))
            {
                var currency = await dbContext.Currencies.FirstOrDefaultAsync(c => c.Name == charCode);

                if (currency == null)
                {
                    dbContext.Currencies.Add(new Currency
                    {
                        Name = charCode,
                        Rate = rate
                    });
                    _logger.LogInformation("Currency added successfully.");
                }
                else
                {
                    currency.Rate = rate;
                    _logger.LogInformation("Currency updated successfully.");
                }
            }
        }

        await dbContext.SaveChangesAsync();
    }
}