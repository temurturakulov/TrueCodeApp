using System.Net;
using System.Security.Cryptography;
using System.Text;
using TrueCodeApp.Core.Domain.Extensions;
using TrueCodeApp.Core.Domain.Models;
using TrueCodeApp.Core.Domain.Services;
using TrueCodeApp.User.Domain.Repositories;
using TrueCodeApp.User.Domain.Services;

namespace TrueCodeApp.User.Infrastructure.Services;

public class AuthService(IUserRepository userRepository, IJwtTokenService jwtTokenService) : IAuthService
{
    public async Task<ServiceResult> RegisterAsync(string login, string password, CancellationToken ct)
    {
        var existing = await userRepository.GetUserByLogin(login, ct);
        if (existing != null)
            return ServiceResult.Failure("User already exists", HttpStatusCode.Conflict);

        var passwordHash = Hash(password);
        await userRepository.AddAsync(
            new Core.Domain.Entities.User { Login = login, Password = passwordHash }, ct);

        return ServiceResult.Success();
    }

    public async Task<ServiceResult<TokenModel>> LoginAsync(string login, string password, CancellationToken ct)
    {
        var user = await userRepository.GetUserByLogin(login, ct);
        
        if (user == null)
            return ServiceResult<TokenModel>.Failure("User not found", HttpStatusCode.NotFound);

        if (user.Password != Hash(password))
            return ServiceResult<TokenModel>.Failure("Invalid password", HttpStatusCode.Unauthorized);

        var token = jwtTokenService.GenerateAccessToken(user);
        return ServiceResult<TokenModel>.Success(token);
    }

    public static string Hash(string password)
    {
        using var sha = SHA256.Create();
        return Convert.ToHexString(sha.ComputeHash(Encoding.UTF8.GetBytes(password)));
    }
}