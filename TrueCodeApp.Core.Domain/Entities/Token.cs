using TrueCodeApp.Core.Domain.Entities.BaseModels;

namespace TrueCodeApp.Core.Domain.Entities;

public class Token : Identity<long>
{
    public long UserId { get; set; }
    public string Value { get; set; } = null!;
    public DateTimeOffset ExpiresAt { get; set; }

    public User User { get; set; } = default!;
}