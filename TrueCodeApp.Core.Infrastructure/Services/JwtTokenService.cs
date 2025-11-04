using System.Globalization;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using TrueCodeApp.Core.Domain.Entities;
using TrueCodeApp.Core.Domain.Models;
using TrueCodeApp.Core.Domain.Services;
using TrueCodeApp.Core.Infrastructure.Configurations;

namespace TrueCodeApp.Core.Infrastructure.Services;

public class JwtTokenService(TimeProvider timeProvider, IOptions<TokenConfigurations> options) : IJwtTokenService
{
    public TokenModel GenerateAccessToken(User user)
    {
        var issued = DateTime.SpecifyKind(timeProvider.GetUtcNow().DateTime, DateTimeKind.Utc);
        var expiration = issued.AddMinutes(options.Value.ExpirationMinutes); 
 
        var tokenHandler = new JwtSecurityTokenHandler();
        
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(
            [
                new Claim("UserId", user.Id.ToString())
            ]),
            Expires = expiration,
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(Encoding.UTF8.GetBytes(options.Value.Key)), SecurityAlgorithms.HmacSha256Signature)
        };

        var accessToken = tokenHandler.CreateToken(tokenDescriptor);
        
        return new TokenModel
        {
            Token = tokenHandler.WriteToken(accessToken),
            IssuedDate = issued,
            ExpirationDate = expiration
        };
    }
}