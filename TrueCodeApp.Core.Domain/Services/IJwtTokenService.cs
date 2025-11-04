using TrueCodeApp.Core.Domain.Entities;
using TrueCodeApp.Core.Domain.Models;

namespace TrueCodeApp.Core.Domain.Services;

public interface IJwtTokenService
{
    TokenModel GenerateAccessToken(User user);
}