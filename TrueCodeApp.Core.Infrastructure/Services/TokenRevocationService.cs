using Microsoft.EntityFrameworkCore;
using TrueCodeApp.Core.Domain.Entities;
using TrueCodeApp.Core.Domain.Services;
using TrueCodeApp.Core.Infrastructure.Persistence;

namespace TrueCodeApp.Core.Infrastructure.Services;

public class TokenRevocationService(ApplicationDbContext dbContext) : ITokenRevocationService
{
    public async Task RevokeAsync(string token, DateTimeOffset expiresAt, long userId, CancellationToken ct)
    {
        if (await dbContext.Tokens.AnyAsync(x => x.Value == token, ct))
            return;

        dbContext.Tokens.Add(new Token
        {
            UserId = userId,
            Value = token,
            ExpiresAt = expiresAt
        });

        await dbContext.SaveChangesAsync(ct);
    }

    public async Task<bool> IsRevokedAsync(string token, CancellationToken ct)
    {
        return await dbContext.Tokens.AnyAsync(x => x.Value == token, ct);
    }
}