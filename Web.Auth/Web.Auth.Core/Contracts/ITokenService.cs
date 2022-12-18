using System.Security.Claims;

namespace Web.Auth.Core.Contracts;

public interface ITokenService
{
    string Generate(Claim[] claims, string key, string issuer, string audience, DateTime expires);

    bool ValidateToken(string token, string key, string issuer, string audience);

    bool TryGetClaimValue(string token, string claimType, out string value);
}