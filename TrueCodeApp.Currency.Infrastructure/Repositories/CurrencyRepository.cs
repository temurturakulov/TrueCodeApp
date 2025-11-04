using System.Net;
using Microsoft.EntityFrameworkCore;
using TrueCodeApp.Core.Domain.Entities;
using TrueCodeApp.Core.Domain.Extensions;
using TrueCodeApp.Core.Infrastructure.Persistence;
using TrueCodeApp.Currency.Domain.Repositories;
using TrueCodeApp.Currency.Infrastructure.Dtos;

namespace TrueCodeApp.Currency.Infrastructure.Repositories;

public class CurrencyRepository(ApplicationDbContext dbContext) : ICurrencyRepository
{
    public async Task<List<CurrencyDto>> GetCurrencyByUserAsync(long userId, CancellationToken ct)
    {
        return await dbContext.UserCurrencies
            .AsNoTracking()
            .Include(x => x.Currency)
            .Where(x => x.UserId == userId)
            .Select(x=>CurrencyDto.MapFromUserCurrency(x))
            .ToListAsync(ct);
    }

    public async Task<List<CurrencyDto>> GetCurrencies(CancellationToken ct)
    {
        return await dbContext.Currencies
            .AsNoTracking()
            .Select(x=>CurrencyDto.MapFromCurrency(x))
            .ToListAsync(ct);
    }

    public async Task<ServiceResult> AddCurrencyAsync(long userId, long currencyId, CancellationToken ct)
    {
        var user = await dbContext.Users
            .FirstOrDefaultAsync(u => u.Id == userId, cancellationToken: ct);

        if (user == null)
            return ServiceResult.Failure("User not found", HttpStatusCode.NotFound);

        var currency = await dbContext.Currencies.FirstOrDefaultAsync(u => u.Id == currencyId, ct);

        if (currency == null)
            return ServiceResult.Failure("Currency not found", HttpStatusCode.NotFound);

        var userCurrency = await dbContext.UserCurrencies
            .FirstOrDefaultAsync(u => u.UserId == userId && u.CurrencyId == currencyId, ct);
 
        if (userCurrency != null) 
            return ServiceResult.Failure("UserCurrency not found", HttpStatusCode.NotFound);

        user.UserCurrencies.Add(new UserCurrency
        {
            UserId = userId,
            CurrencyId = currencyId
        });
        await dbContext.SaveChangesAsync(ct);
        return ServiceResult.Success();
    }

    public async Task<ServiceResult> DeleteCurrencyAsync(long userId, long currencyId, CancellationToken ct)
    {
        var user = await dbContext.Users
            .FirstOrDefaultAsync(u => u.Id == userId, cancellationToken: ct);

        if (user == null) 
            return ServiceResult.Failure("User not found", HttpStatusCode.NotFound);

        var currency = await dbContext.Currencies.FirstOrDefaultAsync(u => u.Id == currencyId, ct);

        if (currency == null)
            return ServiceResult.Failure("Currency not found", HttpStatusCode.NotFound);

        var userCurrency = await dbContext.UserCurrencies
            .FirstOrDefaultAsync(u => u.UserId == userId && u.CurrencyId == currencyId, ct);
 
        if (userCurrency == null) 
            return ServiceResult.Failure("UserCurrency not found", HttpStatusCode.NotFound);

        user.UserCurrencies.Remove(userCurrency);
        await dbContext.SaveChangesAsync(ct);
        
        return ServiceResult.Success();
    }
}