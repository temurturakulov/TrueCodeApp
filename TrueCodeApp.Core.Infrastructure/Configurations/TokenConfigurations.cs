using System.ComponentModel.DataAnnotations;

namespace TrueCodeApp.Core.Infrastructure.Configurations;

public class TokenConfigurations
{
    [Required] public string Key { get; set; } = null!;
    [Required] public string Issuer { get; set; } = null!;
    [Required] public string Audience { get; set; } = null!;
    [Required] public int ExpirationMinutes { get; set; }
}