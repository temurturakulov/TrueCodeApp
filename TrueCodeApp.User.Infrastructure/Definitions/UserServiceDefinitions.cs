using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using TrueCodeApp.Core.Infrastructure.Definitions;
using TrueCodeApp.User.Domain.Repositories;
using TrueCodeApp.User.Domain.Services;
using TrueCodeApp.User.Infrastructure.Repositories;
using TrueCodeApp.User.Infrastructure.Services;

namespace TrueCodeApp.User.Infrastructure.Definitions;

public class UserServiceDefinitions : IAppDefinition
{
    public void ConfigureServices(IServiceCollection services, IConfiguration configuration)
    {
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IAuthService, AuthService>();
    }
}