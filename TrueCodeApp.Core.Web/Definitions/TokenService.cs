using System.IdentityModel.Tokens.Jwt;
using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using TrueCodeApp.Core.Infrastructure.Configurations;

namespace TrueCodeApp.Core.Web.Definitions;

public class TokenService(IOptions<TokenConfigurations> tokenConfig)
{
    public long DecodeAccessToken(string accessToken, bool checkForLifetime)
    {
        if (string.IsNullOrWhiteSpace(accessToken))
            return 0;


        if (string.IsNullOrWhiteSpace(tokenConfig.Value.Key) || 
            string.IsNullOrWhiteSpace(tokenConfig.Value.Audience) || 
            string.IsNullOrWhiteSpace(tokenConfig.Value.Issuer) ||
            tokenConfig.Value.ExpirationMinutes == 0)
            return 0;

        var tokenHandler = new JwtSecurityTokenHandler();
        try
        {
            tokenHandler.ValidateToken(accessToken, new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(tokenConfig.Value.Key)),
                ValidateIssuer = false,
                ValidateAudience = false,
                ClockSkew = TimeSpan.Zero,
                ValidateLifetime = checkForLifetime,
            }, out SecurityToken validatedToken);

            var jwtToken = (JwtSecurityToken)validatedToken;

            if (jwtToken == null)
                return 0;

            var userIdClaim = jwtToken.Claims.FirstOrDefault(c => c.Type == "UserId")?.Value;
            if (string.IsNullOrWhiteSpace(userIdClaim) || !long.TryParse(userIdClaim, out var userId))
                return 0;

            return userId;
        }
        catch
        {
            return 0;
        }
    }
}