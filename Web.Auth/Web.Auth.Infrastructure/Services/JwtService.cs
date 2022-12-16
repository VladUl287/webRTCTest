using System.Text;
using System.Security.Claims;
using Web.Auth.Core.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using Web.Auth.Core.Contracts.Services;

namespace Web.Auth.Infrastructure.Services;

public sealed class JwtService : ITokenService
{
    public string Generate(Claim[] claims, string key, string issuer, string audience, DateTime expires)
    {
        var byteKey = Encoding.UTF8.GetBytes(key);
        var symmetricSecurityKey = new SymmetricSecurityKey(byteKey);

        var credentials = new SigningCredentials(symmetricSecurityKey, SecurityAlgorithms.HmacSha256);

        var securityToken = new JwtSecurityToken(issuer, audience, claims, null, expires, credentials);

        return new JwtSecurityTokenHandler().WriteToken(securityToken);
    }

    public bool ValidateToken(string token, string key, string issuer, string audience)
    {
        try
        {
            var byteKey = Encoding.UTF8.GetBytes(key);
            var symmetricSecurityKey = new SymmetricSecurityKey(byteKey);

            var validationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidIssuer = issuer,
                ValidateAudience = true,
                ValidAudience = audience,
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