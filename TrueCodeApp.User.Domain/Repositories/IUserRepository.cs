namespace TrueCodeApp.User.Domain.Repositories;

public interface IUserRepository
{
    Task<Core.Domain.Entities.User> AddAsync(Core.Domain.Entities.User user, CancellationToken ct);
    Task<Core.Domain.Entities.User?> GetUserByLogin(string login, CancellationToken ct);
}