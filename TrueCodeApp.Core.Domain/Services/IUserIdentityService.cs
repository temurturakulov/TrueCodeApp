using TrueCodeApp.Core.Domain.Extensions;

namespace TrueCodeApp.Core.Domain.Services;

public interface IUserIdentityService
{
    long UserId { get; }
    ServiceResult<string>  GetToken { get; }
}