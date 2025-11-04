using Microsoft.EntityFrameworkCore;
using TrueCodeApp.Core.Infrastructure.Persistence;
using TrueCodeApp.User.Domain.Repositories;

namespace TrueCodeApp.User.Infrastructure.Repositories;

public class UserRepository(ApplicationDbContext db) : IUserRepository
{
    public async Task<Core.Domain.Entities.User> AddAsync(Core.Domain.Entities.User user, CancellationToken ct)
    {
        db.Users.Add(user);
        await db.SaveChangesAsync(ct);
        return user;
    }

    public async Task<Core.Domain.Entities.User?> GetUserByLogin(string login, CancellationToken ct)
    {
        return await db.Users
            .FirstOrDefaultAsync(x => x.Login == login,
                ct);
    }
}