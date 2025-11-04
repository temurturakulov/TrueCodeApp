using System.Net;
using Microsoft.AspNetCore.Http;
using TrueCodeApp.Core.Domain.Extensions;
using TrueCodeApp.Core.Domain.Services;
using TrueCodeApp.Core.Web.Definitions;

namespace TrueCodeApp.Core.Web.Services;

public class UserIdentityService(
    IHttpContextAccessor httpContextAccessor,
    TokenService tokenService) : IUserIdentityService
{
    public long UserId
    {
        get
        {
            if (httpContextAccessor.HttpContext != null)
            {
                return (long)httpContextAccessor.HttpContext!.Items["UserId"];
            }

            return 0;
        }
    }

    public ServiceResult<string> GetToken
    {
        get
        {
            if (!httpContextAccessor.HttpContext.Request.Headers.TryGetValue("Authorization", out var headerValue))
            {
                return ServiceResult<string>.Failure("Missing Authorization header", HttpStatusCode.BadRequest);
            }

            var token = headerValue.FirstOrDefault()?.Split(' ')[1];
            return string.IsNullOrWhiteSpace(token)
                ? ServiceResult<string>.Failure("Token is missing", HttpStatusCode.BadRequest)
                : ServiceResult<string>.Success(token);
        }
    }
}