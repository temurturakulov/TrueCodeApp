namespace TrueCodeApp.Core.Domain.Services;

public interface ITokenRevocationService
{
    Task RevokeAsync(string token, DateTimeOffset expiresAt, long userId, CancellationToken ct);
    Task<bool> IsRevokedAsync(string token, CancellationToken ct);
}