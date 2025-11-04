using TrueCodeApp.Core.Domain.Entities.BaseModels;

namespace TrueCodeApp.Core.Domain.Entities;

public class Currency : Identity<long>
{
    public required string Name { get; set; }
    public required decimal Rate { get; set; }
    public virtual ICollection<UserCurrency> UserCurrencies { get; set; } = [];
}