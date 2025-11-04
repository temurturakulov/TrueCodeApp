using System.IdentityModel.Tokens.Jwt;
using System.Net;
using Microsoft.AspNetCore.Mvc;
using TrueCodeApp.Core.Domain.Services;
using TrueCodeApp.Core.Web.Controllers;
using TrueCodeApp.Core.Web.Definitions;
using TrueCodeApp.User.Domain.Services;
using TrueCodeApp.User.Infrastructure.Models;

namespace TrueCodeApp.User.Web.Controllers;

public class UserController(
    IAuthService authService,
    ITokenRevocationService tokenRevocationService,
    IUserIdentityService userIdentityService) : ApiController
{
    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterRequest request, CancellationToken ct)
    {
        var result = await authService.RegisterAsync(request.Login, request.Password, ct);
        if (!result.IsSuccess)
            return StatusCode((int)(result.StatusCode ?? System.Net.HttpStatusCode.BadRequest),
                new { result.ErrorMessage });

        return Ok();
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest request, CancellationToken ct)
    {
        var result = await authService.LoginAsync(request.Login, request.Password, ct);
        return !result.IsSuccess
            ? StatusCode((int)(result.StatusCode ?? HttpStatusCode.BadRequest), new { result.ErrorMessage })
            : Ok(result.Data);
    }

    [AuthorizeByToken]
    [HttpPost("logout")]
    public async Task<IActionResult> Logout(CancellationToken ct)
    {
        var token = userIdentityService.GetToken;

        if (!token.IsSuccess)
            StatusCode((int)(token.StatusCode ?? HttpStatusCode.BadRequest), new { token.ErrorMessage });

        var userId = userIdentityService.UserId;
        await tokenRevocationService.RevokeAsync(token.Data!, DateTimeOffset.UtcNow, userId, ct);

        return Ok("Token revoked");
    }
}