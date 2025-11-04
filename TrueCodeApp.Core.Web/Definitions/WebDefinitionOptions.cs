using Microsoft.AspNetCore.Http.Json;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace TrueCodeApp.Core.Web.Definitions;

public class WebDefinitionOptions
{
    public bool EnableAuthorize { get; set; }
    public bool EnableSwagger { get; set; }
    public string PathBase { get; set; } = string.Empty;

    public ICollection<string>? ApiVersion { get; set; } = new List<string> { "v1"};
    public Action<SwaggerGenOptions>? ConfigureSwagger { get; set; }

    public Dictionary<string, Action<JsonOptions>>? ConfigureJsonOptions { get; set; }
}