using Microsoft.AspNetCore.Builder;

namespace TrueCodeApp.Core.Web.Definitions;

public abstract class WebAppDefinition
{
    public virtual int OrderIndex => 0;

    /// <summary>
    /// Регистрация сервисов
    /// </summary>
    public abstract void ConfigureServices(WebApplicationBuilder builder);

    /// <summary>
    /// Настройка middleware
    /// </summary>
    public virtual void Configure(WebApplication app) { }
}