namespace TrueCodeApp.Core.Domain.Entities;

public class UserCurrency
{
    public long UserId { get; set; }
    public long CurrencyId { get; set; }

    public User User { get; set; }
    public Currency Currency { get; set; }
}