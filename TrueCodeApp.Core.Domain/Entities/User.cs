using TrueCodeApp.Core.Domain.Entities.BaseModels;

namespace TrueCodeApp.Core.Domain.Entities;

public class User : Identity<long>
{
    public required string Login { get; set; }
    public required string Password { get; set; }

    public virtual ICollection<UserCurrency> UserCurrencies { get; set; } = [];
    public virtual ICollection<Token> Tokens { get; set; } = [];
}