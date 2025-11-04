using System.Net;
using Microsoft.AspNetCore.Mvc;
using TrueCodeApp.Core.Domain.Services;
using TrueCodeApp.Core.Web.Controllers;
using TrueCodeApp.Core.Web.Definitions;
using TrueCodeApp.Currency.Domain.Repositories;

namespace TrueCodeApp.Currency.Web.Controllers;

[AuthorizeByToken]
public class CurrencyController(
    ICurrencyRepository currencyRepository,
    IUserIdentityService userIdentityService) : ApiController
{
    [HttpGet("user-favorites")]
    public async Task<IActionResult> GetUserCurrencies(CancellationToken ct)
    {
        var userId = userIdentityService.UserId;
        var currencies = await currencyRepository.GetCurrencyByUserAsync(userId, ct);
        return Ok(currencies);
    }

    [HttpPost("favorites")]
    public async Task<IActionResult> AddFavorite(long currencyId, CancellationToken ct)
    {
        var userId = userIdentityService.UserId;

        var result = await currencyRepository.AddCurrencyAsync(userId, currencyId, ct);
        return !result.IsSuccess
            ? StatusCode((int)(result.StatusCode ?? HttpStatusCode.BadRequest), new { result.ErrorMessage })
            : Ok(result);
    }

    [HttpDelete("favorites/{currencyId:long}")]
    public async Task<IActionResult> DeleteFavorite(long currencyId, CancellationToken ct)
    {
        var userId = userIdentityService.UserId;

        var result = await currencyRepository.DeleteCurrencyAsync(userId, currencyId, ct);
        return !result.IsSuccess
            ? StatusCode((int)(result.StatusCode ?? HttpStatusCode.BadRequest), new { result.ErrorMessage })
            : Ok(result);
    }

    [HttpGet("currencies")]
    public async Task<IActionResult> GetCurrencies(CancellationToken ct)
    {
        var currencies = await currencyRepository.GetCurrencies(ct);
        return Ok(currencies);
    }
}