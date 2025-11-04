using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using TrueCodeApp.Core.Domain.Services;
using TrueCodeApp.Core.Web.Services;

namespace TrueCodeApp.Core.Web.Definitions;

public class ServiceDefinition : WebAppDefinition
{
    public override void ConfigureServices(WebApplicationBuilder builder)
    {
        builder.Services.AddScoped<IUserIdentityService, UserIdentityService>();
        builder.Services.AddHttpContextAccessor();
        builder.Services.AddTransient<TokenService>();
    }
}