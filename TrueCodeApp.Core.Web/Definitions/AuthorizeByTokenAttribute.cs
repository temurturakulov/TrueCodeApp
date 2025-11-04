using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection;
using TrueCodeApp.Core.Domain.Services;

namespace TrueCodeApp.Core.Web.Definitions;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
public class AuthorizeByTokenAttribute : Attribute, IAsyncAuthorizationFilter
{
    private const string AuthorizationHeader = "Authorization";

    public async Task OnAuthorizationAsync(AuthorizationFilterContext context)
    {
        var allowAnonymous = context.ActionDescriptor.EndpointMetadata
            .OfType<Microsoft.AspNetCore.Authorization.AllowAnonymousAttribute>().Any();
        if (allowAnonymous)
            return;

        var error = new ErrorMessage
        {
            Message = "Unauthorized"
        };

        var unauthorizedResult = new JsonResult(error)
        {
            StatusCode = StatusCodes.Status401Unauthorized
        };

        try
        {
            var httpContext = context.HttpContext;

            if (!httpContext.Request.Headers.TryGetValue(AuthorizationHeader, out var headerValue))
            {
                context.Result = unauthorizedResult;
                return;
            }

            var token = headerValue.FirstOrDefault()?.Split(' ')[1];
            if (string.IsNullOrWhiteSpace(token))
            {
                context.Result = unauthorizedResult;
                return;
            }

            var revokeService = context.HttpContext.RequestServices.GetRequiredService<ITokenRevocationService>();

            var isRevoked = await revokeService.IsRevokedAsync(token, CancellationToken.None);

            if (isRevoked)
            {
                error.Message = "Access token revoked";
                context.Result = unauthorizedResult;
                return;
            }

            var accessTokenService = context.HttpContext.RequestServices.GetRequiredService<TokenService>();

            var userId = accessTokenService.DecodeAccessToken(token, checkForLifetime: true);

            if (userId == 0)
            {
                context.Result = unauthorizedResult;
            }
            
            httpContext.Items["UserId"] = userId;
        }
        catch
        {
            context.Result = unauthorizedResult;
        }
    }
}

public class ErrorMessage
{
    public string Message { get; set; }
}