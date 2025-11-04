using TrueCodeApp.Core.Domain.Entities;
using TrueCodeApp.Core.Domain.Extensions;
using TrueCodeApp.Currency.Infrastructure.Dtos;

namespace TrueCodeApp.Currency.Domain.Repositories;

public interface ICurrencyRepository
{
    Task<List<CurrencyDto>> GetCurrencyByUserAsync(long userId, CancellationToken ct);
    Task<List<CurrencyDto>> GetCurrencies(CancellationToken ct);
    Task<ServiceResult> AddCurrencyAsync(long userId, long currencyId, CancellationToken ct);
    Task<ServiceResult> DeleteCurrencyAsync(long userId, long currencyId, CancellationToken ct);
}