using System.Text;
using System.Security.Claims;
using Web.Auth.Core.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using Web.Auth.Core.Contracts.Services;

namespace Web.Auth.Infrastructure.Services;

public sealed class JwtService : ITokenService
{
    public string Generate(Guid id, string email, TokenConfiguration configuration)
    {
        var byteKey = Encoding.UTF8.GetBytes(configuration.SecurityKey);
        var symmetricSecurityKey = new SymmetricSecurityKey(byteKey);

        var credentials = new SigningCredentials(symmetricSecurityKey, SecurityAlgorithms.HmacSha256);
        var claims = new Claim[]
        {
            new Claim(ClaimTypes.NameIdentifier, id.ToString()),
            new Claim(ClaimTypes.Email, email)
        };

        var issuer = configuration.Issuer;
        var expires = configuration.Expires;
        var audience = configuration.Audience;
        var securityToken = new JwtSecurityToken(issuer, audience, claims, null, expires, credentials);

        return new JwtSecurityTokenHandler().WriteToken(securityToken);
    }

    public bool ValidateToken(string token, TokenConfiguration configuration)
    {
        try
        {
            var byteKey = Encoding.UTF8.GetBytes(configuration.SecurityKey);
            var symmetricSecurityKey = new SymmetricSecurityKey(byteKey);

            var validationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidIssuer = configuration.Issuer,
                ValidateAudience = true,
                ValidAudience = configuration.Audience,
                ValidateLifetime = true,
                ClockSkew = TimeSpan.Zero,
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = symmetricSecurityKey
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            tokenHandler.ValidateToken(token, validationParameters, out SecurityToken validatedToken);

            return true;
        }
        catch
        {
            return false;
        }
    }
}