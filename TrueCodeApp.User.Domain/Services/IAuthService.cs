using TrueCodeApp.Core.Domain.Extensions;
using TrueCodeApp.Core.Domain.Models;

namespace TrueCodeApp.User.Domain.Services;

public interface IAuthService
{
    Task<ServiceResult> RegisterAsync(string login, string password, CancellationToken ct);
    Task<ServiceResult<TokenModel>> LoginAsync(string login, string password, CancellationToken ct);
}