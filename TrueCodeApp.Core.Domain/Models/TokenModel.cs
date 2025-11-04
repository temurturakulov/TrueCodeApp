namespace TrueCodeApp.Core.Domain.Models;

public class TokenModel
{
    public required string Token { get; set; }
    public required DateTimeOffset IssuedDate { get; init; }
    public required DateTimeOffset ExpirationDate { get; init; }
    public TimeSpan TokenTimeSpan => ExpirationDate - IssuedDate;
}