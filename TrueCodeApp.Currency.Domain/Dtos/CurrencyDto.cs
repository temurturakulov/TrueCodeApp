namespace TrueCodeApp.Currency.Infrastructure.Dtos;

public class CurrencyDto
{
    public long Id { get; set; }
    public string Name { get; set; }
    public decimal Rate { get; set; }

    public static CurrencyDto MapFromCurrency(Core.Domain.Entities.Currency currency)
    {
        return new CurrencyDto
        {
            Id = currency.Id,
            Name = currency.Name,
            Rate = currency.Rate
        };
    }
    public static CurrencyDto MapFromUserCurrency(Core.Domain.Entities.UserCurrency currency)
    {
        return new CurrencyDto
        {
            Id = currency.CurrencyId,
            Name = currency.Currency.Name,
            Rate = currency.Currency.Rate
        };
    }
}